using System;
using System.Linq;
using LifxNet;

namespace AlarmClock.Backend.Services
{
    public class OverrideableColorPickingService : ICompositeColorPickingService
    {

        private IBaseColorPickingService _baseColorPicker;
        private IOverrideColorPickingService[] _overrideColorPickers;

        public OverrideableColorPickingService(IBaseColorPickingService baseColorPicker)
        {
            _baseColorPicker = baseColorPicker;
            _overrideColorPickers = Array.Empty<IOverrideColorPickingService>();
        }

        public void AddOverride(IOverrideColorPickingService overrideColorPicker)
        {
            _overrideColorPickers = new[] { overrideColorPicker };
        }

        public void ClearOverride()
        {
            _overrideColorPickers = Array.Empty<IOverrideColorPickingService>();
        }

        public AlarmClockColor GetColorForTime(DateTime time)
        {
            return GetActiveColorPicker(time).GetColorForTime(time);
        }

        public bool IsLightOnAtTime(DateTime time)
        {
            return GetActiveColorPicker(time).IsLightOnAtTime(time);
        }

        public string Status(DateTime time)
        {
            return GetActiveColorPicker(time).Status(time);
        }

        public ICompositeColorPickingService ReconstructWithBase(IBaseColorPickingService baseColorPicker)
        {
            var newColorPicker = new OverrideableColorPickingService(baseColorPicker);
            IOverrideColorPickingService[] overrides = _overrideColorPickers.Clone() as IOverrideColorPickingService[];
            overrides = overrides.Reverse().ToArray();

            //add in reverse order to maintain priority
            foreach (var overridePicker in overrides)
            {
                newColorPicker.AddOverride(overridePicker);
            }
            return newColorPicker;
        }

        /// <summary>
        /// Gets the parameters for the color picking service.
        /// </summary>
        /// <returns></returns>
        public IConfigurableColorPickingServiceParameters GetParameters()
        {
            return _baseColorPicker.GetParameters();
        }

        /// <summary>
        /// Prune expired overrides
        /// </summary>
        /// <param name="currentTime">Prunes overrides that have expired</param>
        private void PruneExpiredOverrides(DateTime currentTime)
        {
            _overrideColorPickers = Array.FindAll(_overrideColorPickers, o => o.EndTime <= currentTime);
        }

        /// <summary>
        /// Get the active color picker for the given time
        /// </summary>
        /// <param name="time">The time to get the color picker for</param>
        /// <returns>The active color picker for the given time</returns>
        private IColorPickingService GetActiveColorPicker(DateTime time)
        {
            PruneExpiredOverrides(time);
            foreach (var overrideColorPicker in _overrideColorPickers)
            {
                if (overrideColorPicker != null &&
                    time >= overrideColorPicker.StartTime)
                {
                    return overrideColorPicker;
                }
            }
            return _baseColorPicker;
        }

    }
}
