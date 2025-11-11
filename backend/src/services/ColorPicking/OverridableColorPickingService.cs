using System;
using System.Collections.Generic;
using System.Linq;
using LifxNet;

namespace AlarmClock.Backend.Services
{
    public class OverrideableColorPickingService : ICompositeColorPickingService
    {

        private IBaseColorPickingService _baseColorPicker;

        private readonly object _overrideLock = new object();
        private List<IOverrideColorPickingService> _overrideColorPickers;

        public OverrideableColorPickingService(IBaseColorPickingService baseColorPicker)
        {
            _baseColorPicker = baseColorPicker;
            _overrideColorPickers = new List<IOverrideColorPickingService>();
        }

        public void AddOverride(IOverrideColorPickingService overrideColorPicker)
        {
            lock (_overrideLock)
            {
                _overrideColorPickers.Add(overrideColorPicker);
            }
        }

        public int GetOverrideCount()
        {
            lock (_overrideLock)
            {
                return _overrideColorPickers.Count;
            }
        }

        public void ClearAllOverrides()
        {
            lock (_overrideLock)
            {
                _overrideColorPickers.Clear();
            }
        }

        public void ClearOverrideByGuid(string overrideGuid)
        {
            lock (_overrideLock)
            {
                _overrideColorPickers = _overrideColorPickers
                    .Where(o => o.guid != overrideGuid)
                    .ToList();
            }
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

        /// <summary>
        /// Reconstructs the composite color picking service with a new base color picker
        /// </summary>
        /// <param name="baseColorPicker">The new base color picker</param>
        /// <returns></returns>
        public ICompositeColorPickingService ReconstructWithBase(IBaseColorPickingService baseColorPicker)
        {
            var newColorPicker = new OverrideableColorPickingService(baseColorPicker);
            List<IOverrideColorPickingService> overrides;
            lock (_overrideLock)
            {
                overrides = [.. _overrideColorPickers];
            }

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
            var clonedBase = _baseColorPicker.Clone(); // Assuming base color picker is immutable or has its own clone method
            var newColorPicker = new OverrideableColorPickingService(clonedBase);
            List<IOverrideColorPickingService> overrides;
            lock (_overrideLock)
            {
                overrides = [.. _overrideColorPickers];
            }

            // Add overrides in the same order to maintain priority
            foreach (var overridePicker in overrides)
            {
                newColorPicker.AddOverride(overridePicker);
            }
            return newColorPicker;
        }

        // Explicit implementation of IBaseColorPickingService.Clone()
        // This is not really necessary since ICompositeColorPickingService inherits from IBaseColorPickingService,
        // but it's included here to satisfy the interface contract.
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
            lock (_overrideLock)
            {
                _overrideColorPickers = _overrideColorPickers.FindAll(o => o.EndTime <= currentTime);
            }
        }

        /// <summary>
        /// Get the active color picker for the given time
        /// </summary>
        /// <param name="time">The time to get the color picker for</param>
        /// <returns>The active color picker for the given time</returns>
        private IColorPickingService GetActiveColorPicker(DateTime time)
        {
            PruneExpiredOverrides(time);
            lock (_overrideLock)
            {
                foreach (var overrideColorPicker in _overrideColorPickers)
                {
                    if (overrideColorPicker != null &&
                        time >= overrideColorPicker.StartTime)
                    {
                        return overrideColorPicker;
                    }
                }
            }
            return _baseColorPicker;
        }

    }
}
