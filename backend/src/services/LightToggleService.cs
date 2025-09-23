using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFullstackApp.Services
{
    public class LightToggleService : BackgroundService, ILightToggleService
    {
        private readonly ILogger<LightToggleService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private bool _isRunning = false;
        private bool _lightState = false; // false = off, true = on
        private const int ToggleIntervalMilliseconds = 5000; // 5000ms = 5 seconds

        public bool IsRunning => _isRunning;

        public LightToggleService(
            ILogger<LightToggleService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Light Toggle Service started");
            _isRunning = true;

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
                using var scope = _serviceScopeFactory.CreateScope();
                var lifxService = scope.ServiceProvider.GetRequiredService<ILifxService>();

                // Toggle the light state
                _lightState = !_lightState;

                await lifxService.SetAllBulbsPowerAsync(_lightState);

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