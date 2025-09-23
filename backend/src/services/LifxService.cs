using Microsoft.Extensions.Logging;
using LifxNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace MyFullstackApp.Services
{
    public class LifxService : ILifxService
    {
        private readonly ILogger<LifxService> _logger;
        private static LifxClient _client = null;
        private static readonly SemaphoreSlim _clientLock = new(1, 1);

        private static int NumberOfBulbs => Bulbs?.Count() ?? 0;
        private static IEnumerable<LightBulb> Bulbs => _client?.Devices.OfType<LightBulb>();
        private static readonly ConcurrentDictionary<string, LightBulb> _bulbs = new();

        public LifxService(ILogger<LifxService> logger)
        {
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await GetOrCreateClientAsync();
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
                        _client = await LifxClient.CreateAsync();
                        _client.DeviceDiscovered += OnDeviceDiscovered;
                        _client.DeviceLost += OnDeviceLost;
                        _client.StartDeviceDiscovery();
                        _logger.LogInformation("LifxClient created and discovery started");

                        // Give some time for initial discovery
                        await Task.Delay(2000);
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