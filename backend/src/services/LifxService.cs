using Microsoft.Extensions.Logging;
using LifxNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net;

namespace MyFullstackApp.Services
{
    public class LifxService : ILifxService
    {
        private readonly ILogger<LifxService> _logger;
        private LifxClient _client = null;
        private readonly SemaphoreSlim _clientLock = new(1, 1);

        private int NumberOfBulbs => Bulbs?.Count() ?? 0;
        private IEnumerable<LightBulb> Bulbs => _client?.Devices.OfType<LightBulb>();
        private readonly ConcurrentDictionary<string, LightBulb> _bulbs = new();

        private const int DiscoveryDelayMilliseconds = 2000;

        public LifxService(ILogger<LifxService> logger)
        {
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Log network interfaces for debugging
            LogNetworkInterfaces();
            await GetOrCreateClientAsync();
        }

        private void LogNetworkInterfaces()
        {
            try
            {
                _logger.LogInformation("Available network interfaces:");
                var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                    .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                foreach (var ni in interfaces)
                {
                    var ipProps = ni.GetIPProperties();
                    var ipv4Addresses = ipProps.UnicastAddresses
                        .Where(addr => addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        .Select(addr => addr.Address.ToString());

                    _logger.LogInformation($"  {ni.Name} ({ni.NetworkInterfaceType}): {string.Join(", ", ipv4Addresses)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enumerate network interfaces");
            }
        }

        private async Task<LifxClient> GetOrCreateClientAsync()
        {
            if (_client == null)
            {
                await _clientLock.WaitAsync();
                try
                {
                    if (_client == null)
                    {
                        _logger.LogInformation("Creating LifxClient...");
                        
                        // Try to create client bound to the 192.168.0.x network interface
                        var homeNetworkInterface = NetworkInterface.GetAllNetworkInterfaces()
                            .Where(ni => ni.OperationalStatus == OperationalStatus.Up)
                            .FirstOrDefault(ni => ni.GetIPProperties().UnicastAddresses
                                .Any(addr => addr.Address.ToString().StartsWith("192.168.0")));

                        if (homeNetworkInterface != null)
                        {
                            var homeNetworkIp = homeNetworkInterface.GetIPProperties().UnicastAddresses
                                .FirstOrDefault(addr => addr.Address.ToString().StartsWith("192.168.0"))?.Address;
                            _logger.LogInformation($"Found home network interface: {homeNetworkInterface.Name} with IP {homeNetworkIp}");
                        }
                        
                        _client = await LifxClient.CreateAsync();
                        _client.DeviceDiscovered += OnDeviceDiscovered;
                        _client.DeviceLost += OnDeviceLost;
                        _client.StartDeviceDiscovery();
                        _logger.LogInformation("LifxClient created and discovery started");

                        // Give some time for initial discovery
                        _logger.LogInformation($"Waiting {DiscoveryDelayMilliseconds}ms for initial device discovery...");
                        await Task.Delay(DiscoveryDelayMilliseconds);

                        var discoveredCount = Bulbs?.Count() ?? 0;
                        _logger.LogInformation($"Initial discovery complete. Found {discoveredCount} LIFX devices");
                    }
                }
                finally
                {
                    _clientLock.Release();
                }
            }
            return _client;
        }

        private void OnDeviceDiscovered(object sender, LifxClient.DeviceDiscoveryEventArgs e)
        {
            if (e.Device is LightBulb bulb)
            {
                var deviceId = bulb.GetHashCode().ToString();
                _logger.LogInformation($"Light bulb discovered: {bulb.ToString()} (ID: {deviceId})");
            }
        }

        private void OnDeviceLost(object sender, LifxClient.DeviceDiscoveryEventArgs e)
        {
            if (e.Device is LightBulb bulb)
            {
                var deviceId = bulb.GetHashCode().ToString();
                _logger.LogInformation($"Light bulb lost: {bulb.ToString()} (ID: {deviceId})");
            }
        }

        public async Task<int> GetNumberOfBulbsAsync()
        {
            try
            {
                await GetOrCreateClientAsync();
                var bulbCount = NumberOfBulbs;
                _logger.LogInformation($"Current number of bulbs: {bulbCount}");
                return bulbCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting number of bulbs");
                throw;
            }
        }

        public async Task<bool> SetAllBulbsPowerAsync(bool powerOn)
        {
            try
            {
                var client = await GetOrCreateClientAsync();

                if (Bulbs.Count() == 0)
                {
                    _logger.LogInformation("No bulbs found to control");
                    return true;
                }

                var tasks = Bulbs.Select(bulb =>
                    client.SetDevicePowerStateAsync(bulb, powerOn)
                );

                await Task.WhenAll(tasks);

                var action = powerOn ? "turned on" : "turned off";
                _logger.LogInformation($"All {Bulbs.Count()} bulbs {action}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting power for all bulbs");
                throw;
            }
        }
    }
}