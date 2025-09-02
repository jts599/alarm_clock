using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;

namespace Backend.Controllers
{
    /// <summary>
    /// WiFi Controller for managing wireless network operations
    /// Provides endpoints for scanning networks, connecting to WiFi, and checking connection status.
    /// Designed specifically for Raspberry Pi captive portal functionality and alarm clock setup.
    /// </summary>
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

        /// <summary>
        /// Scans for available WiFi networks in the area
        /// </summary>
        /// <returns>
        /// Returns a list of available WiFi networks with their details including SSID, signal strength, security type, and frequency.
        /// Networks are sorted by signal strength (strongest first) and duplicates are removed.
        /// </returns>
        /// <response code="200">Successfully scanned and returned available networks</response>
        /// <response code="500">Failed to scan networks due to system error or WiFi interface unavailable</response>
        /// <example>
        /// GET /api/wifi/scan
        /// Returns:
        /// {
        ///   "networks": [
        ///     {
        ///       "ssid": "MyHomeWiFi",
        ///       "signal": -45,
        ///       "security": "WPA/WPA2",
        ///       "frequency": 2442
        ///     }
        ///   ]
        /// }
        /// </example>
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

        /// <summary>
        /// Connects the device to a specified WiFi network
        /// </summary>
        /// <param name="request">WiFi connection details including SSID and password</param>
        /// <returns>
        /// Returns connection status and automatically switches the device from captive portal mode to normal WiFi mode upon successful connection.
        /// The mode switch occurs after a 5-second delay to allow the response to be sent back to the client.
        /// </returns>
        /// <response code="200">Connection attempt completed (check success field in response)</response>
        /// <response code="400">Invalid request - SSID and password are required</response>
        /// <response code="500">Internal server error during connection attempt</response>
        /// <example>
        /// POST /api/wifi/connect
        /// Request Body:
        /// {
        ///   "ssid": "MyHomeWiFi",
        ///   "password": "mySecurePassword123"
        /// }
        /// 
        /// Success Response:
        /// {
        ///   "success": true,
        ///   "message": "Connected successfully"
        /// }
        /// 
        /// Failure Response:
        /// {
        ///   "success": false,
        ///   "message": "Failed to connect to network"
        /// }
        /// </example>
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

        /// <summary>
        /// Gets the current WiFi connection status of the device
        /// </summary>
        /// <returns>
        /// Returns the current connection status including whether the device is connected to a network and the SSID of the connected network if applicable.
        /// Uses iwconfig command to check the current WiFi interface status.
        /// </returns>
        /// <response code="200">Successfully retrieved WiFi connection status</response>
        /// <response code="500">Failed to retrieve status due to system error</response>
        /// <example>
        /// GET /api/wifi/status
        /// 
        /// Connected Response:
        /// {
        ///   "connected": true,
        ///   "ssid": "MyHomeWiFi"
        /// }
        /// 
        /// Disconnected Response:
        /// {
        ///   "connected": false,
        ///   "ssid": ""
        /// }
        /// </example>
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

        /// <summary>
        /// Gets the IP address of the device running the server
        /// </summary>
        /// <returns>
        /// Returns the current IP address of the device, including both WiFi and ethernet interfaces.
        /// Prioritizes WiFi interface IP address but falls back to other network interfaces if needed.
        /// </returns>
        /// <response code="200">Successfully retrieved device IP address</response>
        /// <response code="500">Failed to retrieve IP address due to system error</response>
        /// <example>
        /// GET /api/wifi/ip_address
        /// 
        /// Response:
        /// {
        ///   "ipAddress": "192.168.1.100",
        ///   "interface": "wlan0",
        ///   "success": true
        /// }
        /// </example>
        [HttpGet("ip_address")]
        public async Task<IActionResult> GetIpAddress()
        {
            try
            {
                _logger.LogInformation("Getting device IP address...");

                // Try to get IP from WiFi interface first
                var wifiIp = await GetInterfaceIpAddress(_wifiInterface);
                if (!string.IsNullOrEmpty(wifiIp))
                {
                    return Ok(new
                    {
                        ipAddress = wifiIp,
                        interfaceName = _wifiInterface,
                        success = true
                    });
                }

                // Fallback to any available network interface
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up &&
                                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    .ToList();

                foreach (var networkInterface in networkInterfaces)
                {
                    var ipProperties = networkInterface.GetIPProperties();
                    var ipAddress = ipProperties.UnicastAddresses
                        .FirstOrDefault(ua => ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                                            !IPAddress.IsLoopback(ua.Address))?.Address;

                    if (ipAddress != null)
                    {
                        return Ok(new
                        {
                            ipAddress = ipAddress.ToString(),
                            interfaceName = networkInterface.Name,
                            success = true
                        });
                    }
                }

                // No IP address found
                return Ok(new
                {
                    ipAddress = "",
                    interfaceName = "",
                    success = false,
                    message = "No active network interface found"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device IP address");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Internal server error",
                    message = ex.Message
                });
            }
        }

        private async Task<string> GetInterfaceIpAddress(string interfaceName)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ip",
                        Arguments = $"addr show {interfaceName}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };

                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    // Parse the output to extract IPv4 address
                    var lines = output.Split('\n');
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("inet ") && !trimmedLine.Contains("127.0.0.1"))
                        {
                            var parts = trimmedLine.Split(' ');
                            if (parts.Length > 1)
                            {
                                var ipWithMask = parts[1];
                                var ip = ipWithMask.Split('/')[0];
                                return ip;
                            }
                        }
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting IP for interface {interfaceName}");
                return "";
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

    /// <summary>
    /// Request model for WiFi connection operations
    /// </summary>
    public class WiFiConnectionRequest
    {
        /// <summary>
        /// The SSID (network name) of the WiFi network to connect to
        /// </summary>
        /// <example>MyHomeWiFi</example>
        public string Ssid { get; set; } = "";

        /// <summary>
        /// The password/passphrase for the WiFi network
        /// </summary>
        /// <example>mySecurePassword123</example>
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// Represents a WiFi network discovered during scanning
    /// </summary>
    public class WiFiNetwork
    {
        /// <summary>
        /// The SSID (network name) of the WiFi network
        /// </summary>
        /// <example>MyHomeWiFi</example>
        public string Ssid { get; set; } = "";

        /// <summary>
        /// Signal strength in dBm (typically negative values, closer to 0 is stronger)
        /// </summary>
        /// <example>-45</example>
        public int Signal { get; set; }

        /// <summary>
        /// Security type of the network (e.g., "Open", "WPA/WPA2")
        /// </summary>
        /// <example>WPA/WPA2</example>
        public string Security { get; set; } = "";

        /// <summary>
        /// Operating frequency of the network in MHz
        /// </summary>
        /// <example>2442</example>
        public int Frequency { get; set; }
    }
}
