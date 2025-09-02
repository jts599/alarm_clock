using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LifxNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LifxController : ControllerBase
    {
        private readonly ILogger<LifxController> _logger;
        private static LifxClient _client = null;
        private static readonly ConcurrentDictionary<string, LightBulb> _bulbs = new();
        private static readonly SemaphoreSlim _clientLock = new(1, 1);

        public LifxController(ILogger<LifxController> logger)
        {
            _logger = logger;
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
                _bulbs.TryAdd(deviceId, bulb);
                _logger.LogInformation($"Light bulb discovered: {bulb.ToString()} (ID: {deviceId})");
            }
        }

        private void OnDeviceLost(object sender, LifxClient.DeviceDiscoveryEventArgs e)
        {
            if (e.Device is LightBulb bulb)
            {
                var deviceId = bulb.GetHashCode().ToString();
                _bulbs.TryRemove(deviceId, out _);
                _logger.LogInformation($"Light bulb lost: {bulb.ToString()} (ID: {deviceId})");
            }
        }

        [HttpGet("getNumberOfBulbs")]
        public async Task<IActionResult> GetNumberOfBulbs()
        {
            try
            {
                // Ensure client is initialized
                await GetOrCreateClientAsync();

                var bulbCount = _bulbs.Count;
                _logger.LogInformation($"Current number of bulbs: {bulbCount}");

                return Ok(new
                {
                    numberOfBulbs = bulbCount,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting number of bulbs");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to get number of bulbs",
                    message = ex.Message
                });
            }
        }

        [HttpPost("setAllBulbsPower")]
        public async Task<IActionResult> SetAllBulbsPower([FromBody] SetPowerRequest request)
        {
            try
            {
                var client = await GetOrCreateClientAsync();

                if (_bulbs.IsEmpty)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "No bulbs found to control",
                        bulbsAffected = 0
                    });
                }

                var tasks = _bulbs.Values.Select(bulb =>
                    client.SetDevicePowerStateAsync(bulb, request.On)
                );

                await Task.WhenAll(tasks);

                var action = request.On ? "turned on" : "turned off";
                _logger.LogInformation($"All {_bulbs.Count} bulbs {action}");

                return Ok(new
                {
                    success = true,
                    message = $"All bulbs {action}",
                    bulbsAffected = _bulbs.Count,
                    powerState = request.On
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting power for all bulbs");
                return StatusCode(500, new
                {
                    success = false,
                    error = "Failed to set power for all bulbs",
                    message = ex.Message
                });
            }
        }
    }

    public class SetPowerRequest
    {
        public bool On { get; set; }
    }
}
