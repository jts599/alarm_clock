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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace AlarmClock.Backend.Services
{
    public class StubLifxService : ILifxService
    {
        private readonly ILogger<StubLifxService> _logger;
        private Color? _currentColor = null;
        private ushort _currentKelvin = 0;

        private bool _isOn = false;
        public StubLifxService(ILogger<StubLifxService> logger)
        {
            _logger = logger;
            _logger.LogInformation("StubLifxService initialized - using mock LIFX implementation");
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("StubLifxService: InitializeAsync called");
        }

        public async Task<int> GetNumberOfBulbsAsync()
        {
            _logger.LogDebug("StubLifxService: Returning 2 mock bulbs");
            return 2; // Stub always has 2 bulbs
        }

        public async Task<bool> SetAllBulbsPowerAsync(bool powerOn)
        {
            _isOn = powerOn;
            _logger.LogInformation("StubLifxService: Set all bulbs power to {PowerState}", powerOn);
            return true;
        }


        public async Task<bool> SetColorAllAsync(LifxNet.Color color, ushort kelvin, int _transitionTime = 0)
        {
            _currentColor = color;
            _currentKelvin = kelvin;
            _logger.LogInformation("StubLifxService: Set color to R:{R} G:{G} B:{B}, Kelvin:{Kelvin}",
                color.R, color.G, color.B, kelvin);
            return true;
        }

        public async Task RefreshBulbStatesAsync()
        {
            _logger.LogDebug("StubLifxService: RefreshBulbStatesAsync called - no action in stub");
        }
    }
}

#pragma warning restore CS1998