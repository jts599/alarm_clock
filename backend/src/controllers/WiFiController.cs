using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WiFiController : ControllerBase
    {
        private readonly ILogger<WiFiController> _logger;
        private readonly string _wifiInterface;

        public WiFiController(ILogger<WiFiController> logger)
        {
            _logger = logger;
            _wifiInterface = Environment.GetEnvironmentVariable("WIFI_INTERFACE") ?? "wlan0";
        }

        [HttpGet("scan")]
        public async Task<IActionResult> ScanNetworks()
        {
            try
            {
                _logger.LogInformation("Scanning for WiFi networks...");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = $"iwlist {_wifiInterface} scan",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    _logger.LogError($"WiFi scan failed: {error}");
                    return StatusCode(500, new { error = "Failed to scan networks" });
                }

                var networks = ParseScanOutput(output);
                return Ok(new { networks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scanning WiFi networks");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("connect")]
        public async Task<IActionResult> ConnectToNetwork([FromBody] WiFiConnectionRequest request)
        {
            try
            {
                _logger.LogInformation($"Attempting to connect to network: {request.Ssid}");

                // Validate input
                if (string.IsNullOrEmpty(request.Ssid) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { success = false, message = "SSID and password are required" });
                }

                // Create wpa_supplicant configuration
                var success = await ConfigureAndConnectWiFi(request.Ssid, request.Password);

                if (success)
                {
                    _logger.LogInformation("WiFi configured successfully, scheduling mode switch...");

                    // Schedule the switch back to normal mode (give time for response to be sent)
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(5000); // Wait 5 seconds
                        await SwitchToNormalMode();
                    });

                    return Ok(new { success = true, message = "Connected successfully" });
                }
                else
                {
                    return Ok(new { success = false, message = "Failed to connect to network" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to WiFi network");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetConnectionStatus()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "iwconfig",
                        Arguments = _wifiInterface,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                var isConnected = output.Contains("ESSID:") && !output.Contains("ESSID:off");
                var ssid = ExtractSSIDFromStatus(output);

                return Ok(new { connected = isConnected, ssid = ssid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting WiFi status");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        private List<WiFiNetwork> ParseScanOutput(string output)
        {
            var networks = new List<WiFiNetwork>();
            var lines = output.Split('\n');

            WiFiNetwork currentNetwork = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("Cell"))
                {
                    if (currentNetwork != null)
                        networks.Add(currentNetwork);
                    currentNetwork = new WiFiNetwork();
                }
                else if (currentNetwork != null)
                {
                    if (trimmedLine.StartsWith("ESSID:"))
                    {
                        var ssid = trimmedLine.Substring(7).Trim('"');
                        if (!string.IsNullOrEmpty(ssid) && ssid != "<hidden>")
                            currentNetwork.Ssid = ssid;
                    }
                    else if (trimmedLine.StartsWith("Signal level="))
                    {
                        var signalPart = trimmedLine.Split('=')[1].Split(' ')[0];
                        if (int.TryParse(signalPart, out var signal))
                            currentNetwork.Signal = signal;
                    }
                    else if (trimmedLine.Contains("Encryption key:"))
                    {
                        currentNetwork.Security = trimmedLine.Contains("on") ? "WPA/WPA2" : "Open";
                    }
                    else if (trimmedLine.StartsWith("Frequency:"))
                    {
                        var freqPart = trimmedLine.Split(':')[1].Split(' ')[0];
                        if (double.TryParse(freqPart, out var freq))
                            currentNetwork.Frequency = (int)(freq * 1000);
                    }
                }
            }

            if (currentNetwork != null && !string.IsNullOrEmpty(currentNetwork.Ssid))
                networks.Add(currentNetwork);

            return networks.Where(n => !string.IsNullOrEmpty(n.Ssid))
                          .GroupBy(n => n.Ssid)
                          .Select(g => g.First())
                          .OrderByDescending(n => n.Signal)
                          .ToList();
        }

        private async Task<bool> ConfigureAndConnectWiFi(string ssid, string password)
        {
            try
            {
                // Create wpa_supplicant configuration entry
                var wpaConfig = $@"
network={{
    ssid=""{ssid}""
    psk=""{password}""
    key_mgmt=WPA-PSK
}}";

                // Write to temporary file
                var tempFile = "/tmp/new_wifi_config";
                await System.IO.File.WriteAllTextAsync(tempFile, wpaConfig);

                // Update wpa_supplicant configuration
                var updateProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = $"tee -a /etc/wpa_supplicant/wpa_supplicant.conf < {tempFile}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                updateProcess.Start();
                await updateProcess.WaitForExitAsync();

                // Clean up temp file
                System.IO.File.Delete(tempFile);

                return updateProcess.ExitCode == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring WiFi");
                return false;
            }
        }

        private async Task SwitchToNormalMode()
        {
            try
            {
                _logger.LogInformation("Switching to normal WiFi mode...");

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "sudo",
                        Arguments = "/home/pi/alarm_clock/raspberry-pi/disable-captive-portal.sh",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                await process.WaitForExitAsync();

                _logger.LogInformation($"Mode switch completed with exit code: {process.ExitCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error switching to normal mode");
            }
        }

        private string ExtractSSIDFromStatus(string output)
        {
            var essidLine = output.Split('\n').FirstOrDefault(l => l.Contains("ESSID:"));
            if (essidLine != null)
            {
                var start = essidLine.IndexOf("ESSID:") + 6;
                var end = essidLine.Length;
                return essidLine.Substring(start, end - start).Trim().Trim('"');
            }
            return "";
        }
    }

    public class WiFiConnectionRequest
    {
        public string Ssid { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class WiFiNetwork
    {
        public string Ssid { get; set; } = "";
        public int Signal { get; set; }
        public string Security { get; set; } = "";
        public int Frequency { get; set; }
    }
}
