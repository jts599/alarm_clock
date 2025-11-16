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


namespace AlarmClock.Backend.Services
{
    public class LifxService : ILifxService
    {
        private readonly ILogger<LifxService> _logger;
        private LifxClient _client = null;
        private readonly SemaphoreSlim _clientCreationLock = new(1, 1);

        private readonly SemaphoreSlim _clientLock = new(1, 1);

        private int NumberOfBulbs => Bulbs?.Count() ?? 0;
        private IEnumerable<LightBulb> Bulbs => _client?.Devices.OfType<LightBulb>();

        private const int DiscoveryDelayMilliseconds = 2500;


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
                await _clientCreationLock.WaitAsync();
                try
                {
                    if (_client == null)
                    {
                        _logger.LogInformation("Creating LifxClient...");

                        _client = await LifxClient.CreateAsync();
                        _logger.LogInformation("LifxClient created successfully");

                        _client.DeviceDiscovered += OnDeviceDiscovered;
                        _client.DeviceLost += OnDeviceLost;

                        _logger.LogInformation("Starting device discovery...");
                        await _client.StartDeviceDiscovery(false); //This will have a long enough delay internally to discover devices
                        _client.StopDeviceDiscovery();

                        var discoveredCount = Bulbs?.Count() ?? 0;
                        _logger.LogInformation($"Initial discovery complete. Found {discoveredCount} LIFX devices. They will refresh every minute.");
                        //await FlashFoundBulbs();
                    }
                }
                finally
                {
                    _clientCreationLock.Release();
                }
            }
            return _client;
        }

        //This is the lock that should be used to protect access to the client
        private SemaphoreSlim ClientLock => _clientLock;

        //This should really be the only method that gets the client. That way you can use the client lock provided by _clientCreationLock
        private Task<LifxClient> GetClientAsync()
        {
            if (_client == null)
            {
                //Creation may be in progress, wait for it if it is
                _clientCreationLock.Wait();
                _clientCreationLock.Release();
                //Check again
                if (_client == null)
                {
                    //Still null, means it was never created
                    throw new InvalidOperationException("LifxClient has not been initialized. Call InitializeAsync() first.");
                }
            }
            return Task.FromResult(_client);
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
            await ClientLock.WaitAsync();
            try
            {
                await GetClientAsync();
                var bulbCount = NumberOfBulbs;
                return bulbCount;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting number of bulbs: {Message}", e.Message);
                throw;
            }
            finally
            {
                ClientLock.Release();
            }

        }

        public async Task<bool> SetAllBulbsPowerAsync(bool powerOn)
        {
            await ClientLock.WaitAsync();
            try
            {
                var client = await GetClientAsync();
                if (Bulbs.Count() == 0)
                {
                    return true;
                }

                var tasks = Bulbs.Select(bulb =>
                    client.SetDevicePowerStateAsync(bulb, powerOn)
                );

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting power for all bulbs");
                throw;
            }
            finally
            {
                ClientLock.Release();
            }
        }

        public async Task RefreshBulbStatesAsync()
        {
            await ClientLock.WaitAsync();
            try
            {
                _logger.LogInformation("Refreshing available bulbs...");
                var client = await GetClientAsync();
                await client.RefreshDevicesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing bulb states");
                throw;
            }
            finally
            {
                ClientLock.Release();
            }
        }


        public async Task<bool> SetColorAllAsync(LifxNet.Color color, ushort kelvin, int transitionTime = 0)
        {
            await ClientLock.WaitAsync();
            try
            {
                var client = await GetClientAsync();
                if (Bulbs.Count() == 0)
                {
                    return true;
                }
                Bulbs.ToList().ForEach(async bulb =>
                {
                    //I am afraid of using Task.WhenAll here because LifxNet might not support multiple concurrent commands well
                    await client.SetColorAsync(bulb, color, kelvin, TimeSpan.FromSeconds(transitionTime));
                });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting color for all bulbs");
                throw;
            }
            finally
            {
                ClientLock.Release();
            }
        }
    }
}