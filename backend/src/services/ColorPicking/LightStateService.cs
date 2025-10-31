using AlarmClock.Backend.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public class LightStateService : BackgroundService, ILightStateService
    {
        private readonly ILogger<LightStateService> _logger;
        private readonly ILifxService _lifxService;

        private readonly RunConfiguration _config;

        private bool _lastSetOnState = false; // false = off, true = on
        private DateTime _startTime = DateTime.MinValue;

        private int _secondsStepInterval = 1; // 1 second
        private AlarmClockColor _lastSetColor = null;

        private ICompositeColorPickingService _colorPicker;
        private readonly object _colorPickerLock = new object();

        public void SwapColorPicker(ICompositeColorPickingService newColorPicker)
        {
            lock (_colorPickerLock)
            {
                _colorPicker = newColorPicker;
            }
        }

        public void SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker)
        {
            lock (_colorPickerLock)
            {
                _colorPicker = _colorPicker?.ReconstructWithBase(newBaseColorPicker) ??
                              new OverrideableColorPickingService(newBaseColorPicker);
            }
        }

        public AlarmClockColor GetCurrentColor()
        {
            DateTime scaledNow = GetScaledTime();
            lock (_colorPickerLock)
            {
                return _colorPicker?.GetColorForTime(scaledNow) ?? AlarmClockColor.Default;
            }
        }

        public bool isLightCurrentlyOn()
        {
            lock (_colorPickerLock)
            {
                return _colorPicker?.IsLightOnAtTime(GetScaledTime()) ?? false;
            }
        }

        public IConfigurableColorPickingServiceParameters GetCurrentParameters()
        {
            lock (_colorPickerLock)
            {
                return _colorPicker?.GetParameters() ?? throw new InvalidOperationException("Color picker not initialized");
            }
        }

        public LightStateService(
            ILogger<LightStateService> logger,
            ILifxService lifxService,
            ICompositeColorPickingService colorPicker,
            IOptions<RunConfiguration> configOptions,
            DateTime? startTime = null
            )
        {
            var config = configOptions.Value;
            int secondsStepInterval = config.TimescaleMultiplier;
            _config = config;
            _logger = logger;
            _lifxService = lifxService;
            _colorPicker = colorPicker;
            _secondsStepInterval = secondsStepInterval;
            _startTime = startTime ?? DefaultStartTime;
            _trueStartTime = DateTime.Now;
        }

        private DateTime _trueStartTime;

        /// <summary>
        /// In prod this should be DateTime.Now, for testing it can be set to a fixed time.
        /// </summary>
        private DateTime DefaultStartTime => AlarmStartTime;


        /// <summary>
        /// This can be used for testing so that the alarm goes off when the program starts. 
        /// Update DefaultStartTime to change the default alarm start time.
        /// </summary>
        private DateTime AlarmStartTime
        {
            get
            {
                if (_config != null && _config.StartTimeHour >= 0 && _config.StartTimeHour < 24)
                {
                    return DateTime.Today.AddHours(_config.StartTimeHour);
                }
                return DateTime.Today.AddHours(6).AddMinutes(30); // 6:30 AM today
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Light State Service started");

            // Initialize the LIFX service
            await _lifxService.InitializeAsync();

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await UpdateLightStateAsync();
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

        public DateTime GetScaledTime()
        {
            DateTime now = DateTime.Now;
            int secondsSinceStart = (int)(now - _trueStartTime).TotalSeconds;
            int scaledSecondsSinceStart = secondsSinceStart * _secondsStepInterval;
            DateTime scaledNow = _startTime.AddSeconds(scaledSecondsSinceStart);
            return scaledNow;
        }

        private async Task UpdateLightStateAsync()
        {
            try
            {
                DateTime now = DateTime.Now;
                int secondsSinceStart = (int)(now - _trueStartTime).TotalSeconds;
                int scaledSecondsSinceStart = secondsSinceStart * _secondsStepInterval;
                DateTime scaledNow = GetScaledTime();
                //_logger.LogInformation($"Scaled time: {scaledNow}");

                AlarmClockColor desiredColor;
                bool desiredOnState;

                bool shouldUpdateColor;
                bool shouldUpdateOnState;
                int transitionTime = 0;

                lock (_colorPickerLock)
                {
                    desiredColor = _colorPicker.GetColorForTime(scaledNow);
                    desiredOnState = _colorPicker.IsLightOnAtTime(scaledNow);
                    shouldUpdateColor = IsChosenColorDifferent(desiredColor);
                    shouldUpdateOnState = IsChosenOnStateDifferent(desiredOnState);
                    if (shouldUpdateColor)
                    {
                        transitionTime = CalculateTransitionTime(desiredColor, scaledNow);
                    }
                }
                if (IsChosenColorDifferent(desiredColor))
                {
                    _logger.LogInformation($"Changing light color to: {desiredColor}. Transition time: {transitionTime} seconds");
                    await _lifxService.SetColorAllAsync(desiredColor.Color, desiredColor.Kelvin, transitionTime);
                    _lastSetColor = desiredColor;

                }

                if (IsChosenOnStateDifferent(desiredOnState))
                {
                    _logger.LogInformation($"Changing light on state to: {desiredOnState}");
                    await _lifxService.SetAllBulbsPowerAsync(desiredOnState);
                    _lastSetOnState = desiredOnState;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Setting Lights");
            }
        }

        private int CalculateTransitionTime(AlarmClockColor newColor, DateTime scaledNow)
        {
            int time = 0;
            DateTime checkTime = scaledNow;
            do
            {
                time += 1;
                checkTime = scaledNow.AddSeconds(time);

                var nextColor = _colorPicker.GetColorForTime(checkTime);
                if (!newColor.IsEqual(nextColor))
                {
                    break;
                }
            } while (time < 10); // Limit to 10 seconds max transition time
            return time;
        }

        private bool IsChosenColorDifferent(AlarmClockColor newColor)
        {
            if (_lastSetColor == null)
                return true;
            return !_lastSetColor.IsEqual(newColor);
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