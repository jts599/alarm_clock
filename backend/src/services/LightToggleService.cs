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
        private int _secondsCounter = 0;
        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning => _isRunning;

        public LightToggleService(
            ILogger<LightToggleService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Light Toggle Service started");
            _isRunning = true;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _secondsCounter++;

                    // Toggle lights every 5 seconds
                    if (_secondsCounter >= 5)
                    {
                        await ToggleLightsAsync();
                        _secondsCounter = 0;
                    }

                    // Wait 1 second before next iteration
                    await Task.Delay(1000, stoppingToken);
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

        public async Task StartAsync()
        {
            if (!_isRunning)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await StartAsync(_cancellationTokenSource.Token);
            }
        }

        public async Task StopAsync()
        {
            if (_isRunning)
            {
                _cancellationTokenSource.Cancel();
                await Task.Delay(100); // Give it a moment to stop gracefully
            }
        }

        public override void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            base.Dispose();
        }
    }
}