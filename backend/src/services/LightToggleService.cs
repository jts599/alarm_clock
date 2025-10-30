using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public class LightToggleService : BackgroundService, ILightToggleService
    {
        private readonly ILogger<LightToggleService> _logger;
        private readonly ILifxService _lifxService;
        private bool _isRunning = false;
        private bool _lightState = false; // false = off, true = on
        private const int ToggleIntervalMilliseconds = 1000; // 1000ms = 1 second

        public bool IsRunning => _isRunning;

        public LightToggleService(
            ILogger<LightToggleService> logger,
            ILifxService lifxService)
        {
            _logger = logger;
            _lifxService = lifxService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Light Toggle Service started");
            _isRunning = true;

            // Initialize the LIFX service
            await _lifxService.InitializeAsync();

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(ToggleIntervalMilliseconds));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await ToggleLightsAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Light Toggle Service was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Light Toggle Service");
            }
            finally
            {
                _isRunning = false;
                _logger.LogInformation("Light Toggle Service stopped");
            }
        }

        private async Task ToggleLightsAsync()
        {
            try
            {
                // Toggle the light state
                _lightState = !_lightState;

                await _lifxService.SetAllBulbsPowerAsync(_lightState);

                var action = _lightState ? "on" : "off";
                _logger.LogInformation($"Toggled all lights {action}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling lights");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}