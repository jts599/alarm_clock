using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Backend.Controllers
{
    /// <summary>
    /// Network Information Controller for retrieving device network status
    /// Provides endpoints for checking WiFi connection status and device IP address
    /// </summary>
    [ApiController]
    [Route("api/wifi")]
    public class NetworkInfoController : ControllerBase
    {
        private readonly ILogger<NetworkInfoController> _logger;
        private readonly string _wifiInterface;

        public NetworkInfoController(ILogger<NetworkInfoController> logger)
        {
            _logger = logger;
            _wifiInterface = Environment.GetEnvironmentVariable("WIFI_INTERFACE") ?? "wlan0";
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
}
