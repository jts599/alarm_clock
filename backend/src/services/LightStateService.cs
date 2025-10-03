using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyFullstackApp.Services
{
    public class LightStateService : BackgroundService, ILightStateService
    {
        private readonly ILogger<LightStateService> _logger;
        private readonly ILifxService _lifxService;
        private bool _lastSetOnState = false; // false = off, true = on
        private DateTime _startTime = DateTime.MinValue;

        private int _secondsStepInterval = 1; // 1 second
        private AlarmClockColor _lastSetColor = null;

        private IColorPickingService _colorPicker;
        private readonly object _colorPickerLock = new object();

        public void SwapColorPicker(IColorPickingService newColorPicker)
        {
            lock (_colorPickerLock)
            {
                _colorPicker = newColorPicker;
            }
        }

        public LightStateService(
            ILogger<LightStateService> logger,
            ILifxService lifxService,
            IColorPickingService colorPicker,
            int secondsStepInterval = 3600
            )
        {
            _logger = logger;
            _lifxService = lifxService;
            _colorPicker = colorPicker;
            _secondsStepInterval = secondsStepInterval;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Light State Service started");
            _startTime = DateTime.Now;

            // Initialize the LIFX service
            await _lifxService.InitializeAsync();

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await ToggleLightsAsync();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Light State Service was cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Light State Service");
            }
            finally
            {
                _logger.LogInformation("Light State Service stopped");
            }
        }

        private async Task ToggleLightsAsync()
        {
            try
            {
                DateTime now = DateTime.Now;
                int secondsSinceStart = (int)(now - _startTime).TotalSeconds;
                int scaledSecondsSinceStart = secondsSinceStart * _secondsStepInterval;
                DateTime scaledNow = _startTime.AddSeconds(scaledSecondsSinceStart);
                _logger.LogInformation($"Scaled time: {scaledNow}");

                AlarmClockColor desiredColor;
                bool desiredOnState;

                lock (_colorPickerLock)
                {
                    desiredColor = _colorPicker.GetColorForTime(scaledNow);
                    desiredOnState = _colorPicker.IsLightOnAtTime(scaledNow);
                }
                if (IsChosenOnStateDifferent(desiredOnState))
                {
                    _logger.LogInformation($"Changing light on state to: {desiredOnState}");
                    await _lifxService.SetAllBulbsPowerAsync(desiredOnState);
                    _lastSetOnState = desiredOnState;
                }

                if (IsChosenColorDifferent(desiredColor))
                {
                    _logger.LogInformation($"Changing light color to: {desiredColor}");
                    await _lifxService.SetColorAllAsync(desiredColor.Color, desiredColor.Kelvin);
                    _lastSetColor = desiredColor;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Setting Lights");
            }
        }

        private bool IsChosenColorDifferent(AlarmClockColor newColor)
        {
            if (_lastSetColor == null)
                return true;
            return !_lastSetColor.IsEqual((IAlarmClockColor)newColor);
        }

        private bool IsChosenOnStateDifferent(bool newOnState)
        {
            return _lastSetOnState != newOnState;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}