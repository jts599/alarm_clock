using System;
using System.Collections.Generic;
using System.Linq;
using AlarmClock.Backend.DataModels.AlarmCore;
using LifxNet;

namespace AlarmClock.Backend.Services
{
    /// <summary>
    /// NOT THREAD-SAFE. Caller must handle synchronization.
    /// This class assumes it will only be accessed by one thread at a time.
    /// </summary>
    public class OverrideableColorPickingService : ICompositeColorPickingService
    {
        private IBaseColorPickingService _baseColorPicker;
        private List<IOverrideColorPickingService> _overrideColorPickers;

        public OverrideableColorPickingService(IBaseColorPickingService baseColorPicker)
        {
            _baseColorPicker = baseColorPicker;
            _overrideColorPickers = new List<IOverrideColorPickingService>();
        }

        public void AddOverride(IOverrideColorPickingService overrideColorPicker)
        {
            _overrideColorPickers.Add(overrideColorPicker);
        }

        public int GetOverrideCount()
        {
            return _overrideColorPickers.Count;
        }

        public void ClearAllOverrides()
        {
            _overrideColorPickers.Clear();
        }

        public void ClearOverrideByGuid(string overrideGuid)
        {
            _overrideColorPickers = _overrideColorPickers
                .Where(o => o.guid != overrideGuid)
                .ToList();
        }

        public AlarmClockColor GetColorForTime(DateTime time)
        {
            return GetActiveColorPicker(time).GetColorForTime(time);
        }

        public bool IsLightOnAtTime(DateTime time)
        {
            return GetActiveColorPicker(time).IsLightOnAtTime(time);
        }

        public AlarmEventInfo NextEvent(DateTime time)
        {
            return GetActiveColorPicker(time).NextEvent(time);
        }

        /// <summary>
        /// Reconstructs the composite color picking service with a new base color picker
        /// </summary>
        /// <param name="baseColorPicker">The new base color picker</param>
        /// <returns></returns>
        public ICompositeColorPickingService ReconstructWithBase(IBaseColorPickingService baseColorPicker)
        {
            var newColorPicker = new OverrideableColorPickingService(baseColorPicker);
            var overrides = new List<IOverrideColorPickingService>(_overrideColorPickers);
            overrides.Reverse();

            //add in reverse order to maintain priority
            foreach (var overridePicker in overrides)
            {
                newColorPicker.AddOverride(overridePicker);
            }
            return newColorPicker;
        }

        public ICompositeColorPickingService Clone()
        {
            var clonedBase = _baseColorPicker.Clone();
            var newColorPicker = new OverrideableColorPickingService(clonedBase);

            // Add overrides in the same order to maintain priority
            foreach (var overridePicker in _overrideColorPickers)
            {
                newColorPicker.AddOverride(overridePicker);
            }
            return newColorPicker;
        }

        // Explicit implementation of IBaseColorPickingService.Clone()
        IBaseColorPickingService IBaseColorPickingService.Clone()
        {
            return _baseColorPicker.Clone();
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
            _overrideColorPickers = _overrideColorPickers.FindAll(o => o.EndTime > currentTime);
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
