using AlarmClock.Backend.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public class LightStateService : BackgroundService, ILightStateService
    {
        private readonly ILogger<LightStateService> _logger;
        private readonly ILifxService _lifxService;

        private readonly RunConfiguration _config;

        private volatile bool _lastSetOnState = false; // false = off, true = on
        private DateTime _startTime = DateTime.MinValue;

        private int _secondsStepInterval = 1; // 1 second default. overridden by config
        private volatile AlarmClockColor _lastSetColor = null;

        private ICompositeColorPickingService _colorPicker;
        private readonly object _colorPickerLock = new object();

        private IAlarmTimeConfigurationService _alarmConfigService;

        public LightStateService(
            ILogger<LightStateService> logger,
            ILifxService lifxService,
            ICompositeColorPickingService colorPicker,
            IOptions<RunConfiguration> configOptions,
            IAlarmTimeConfigurationService alarmConfigService,
            DateTime? startTime = null
            )
        {
            var config = configOptions.Value;
            int secondsStepInterval = config.TimescaleMultiplier > 0 ? config.TimescaleMultiplier : 1;
            _config = config;
            _logger = logger;
            _lifxService = lifxService;
            _colorPicker = colorPicker;
            _secondsStepInterval = secondsStepInterval;
            _startTime = startTime ?? InternalClockStartTime;
            _alarmConfigService = alarmConfigService;
            _trueStartTime = DateTime.Now;
        }

        public Task SwapColorPicker(ICompositeColorPickingService newColorPicker)
        {
            lock (_colorPickerLock)
            {
                _colorPicker = newColorPicker;
            }
            return Task.CompletedTask;
        }

        public async Task SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker)
        {
            lock (_colorPickerLock)
            {
                _colorPicker = _colorPicker?.ReconstructWithBase(newBaseColorPicker) ??
                              new OverrideableColorPickingService(newBaseColorPicker);
            }

            // This I/O operation happens outside the lock
            IConfigurableColorPickingServiceParameters newParameters = newBaseColorPicker.GetParameters();
            await ConfigurableColorPickingServiceConstructionParameters.SaveToAlarmTimeConfigurationService(
                (AlarmTimeConfigurationService)_alarmConfigService,
                newParameters);
        }

        public Task<ICompositeColorPickingService> GetCurrentColorPickerCopy()
        {
            lock (_colorPickerLock)
            {
                return Task.FromResult(_colorPicker?.Clone());
            }
        }

        public Task<string> AddOverride(DateTime endTime, AlarmClockColor color = null)
        {
            lock (_colorPickerLock)
            {
                var overrideService = new LightOnOverrideColorPickingService(endTime, color);
                _colorPicker?.AddOverride(overrideService);
                return Task.FromResult(overrideService.guid);
            }
        }

        public Task<bool> RemoveOverride(string guid)
        {
            lock (_colorPickerLock)
            {
                var countBefore = _colorPicker?.GetOverrideCount() ?? 0;
                _colorPicker?.ClearOverrideByGuid(guid);
                var countAfter = _colorPicker?.GetOverrideCount() ?? 0;
                return Task.FromResult(countBefore != countAfter);
            }
        }

        public Task ClearAllOverrides()
        {
            lock (_colorPickerLock)
            {
                _colorPicker?.ClearAllOverrides();
            }
            return Task.CompletedTask;
        }

        public Task<int> GetOverrideCount()
        {
            lock (_colorPickerLock)
            {
                return Task.FromResult(_colorPicker?.GetOverrideCount() ?? 0);
            }
        }

        public async Task<AlarmClockColor> GetCurrentColor()
        {
            DateTime scaledNow = await GetScaledTime();
            lock (_colorPickerLock)
            {
                return _colorPicker?.GetColorForTime(scaledNow) ?? AlarmClockColor.Default;
            }
        }

        public async Task<bool> IsLightCurrentlyOn()
        {
            DateTime scaledNow = await GetScaledTime();
            lock (_colorPickerLock)
            {
                return _colorPicker?.IsLightOnAtTime(scaledNow) ?? false;
            }
        }

        public Task<IConfigurableColorPickingServiceParameters> GetCurrentParameters()
        {
            lock (_colorPickerLock)
            {
                return Task.FromResult(_colorPicker?.GetParameters() ?? throw new InvalidOperationException("Color picker not initialized"));
            }
        }

        private DateTime _trueStartTime;

        /// <summary>
        /// This will read from config to see if the start time has been overridden.
        /// It will return that time, or DateTime.Now if not set.
        /// </summary>
        private DateTime InternalClockStartTime
        {
            get
            {
                if (_config != null && !string.IsNullOrEmpty(_config.StartTimeIso8601))
                {
                    try
                    {
                        DateTime parsedTime = DateTime.Parse(_config.StartTimeIso8601);
                        return parsedTime;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error parsing StartTimeIso8601 from config. Falling back to DateTime.Now");
                    }
                }
                return DateTime.Now;
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

        public Task<DateTime> GetScaledTime()
        {
            DateTime now = DateTime.Now;
            int secondsSinceStart = (int)(now - _trueStartTime).TotalSeconds;
            int scaledSecondsSinceStart = secondsSinceStart * _secondsStepInterval;
            DateTime scaledNow = _startTime.AddSeconds(scaledSecondsSinceStart);
            return Task.FromResult(scaledNow);
        }

        private async Task UpdateLightStateAsync()
        {
            try
            {
                DateTime now = DateTime.Now;
                int secondsSinceStart = (int)(now - _trueStartTime).TotalSeconds;
                bool forceUpdate = false;
                if (secondsSinceStart % 60 == 0)
                {
                    //Once a minute refresh bulb states
                    await _lifxService.RefreshBulbStatesAsync();
                    forceUpdate = true;
                }

                int scaledSecondsSinceStart = secondsSinceStart * _secondsStepInterval;
                DateTime scaledNow = await GetScaledTime();
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
                if (IsChosenColorDifferent(desiredColor) || forceUpdate)
                {
                    _logger.LogInformation($"Changing light color to: {desiredColor}. Transition time: {transitionTime} seconds");
                    await _lifxService.SetColorAllAsync(desiredColor.Color, desiredColor.Kelvin, transitionTime);
                    _lastSetColor = desiredColor;

                }

                if (IsChosenOnStateDifferent(desiredOnState) || forceUpdate)
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