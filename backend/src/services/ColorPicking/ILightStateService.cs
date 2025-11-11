using System;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        Task SwapColorPicker(ICompositeColorPickingService newColorPicker);
        Task SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker);
        Task<ICompositeColorPickingService> GetCurrentColorPickerCopy();

        /// <summary>
        /// Add a light override that keeps the light on until the specified time
        /// </summary>
        /// <param name="endTime">When the override should end</param>
        /// <param name="color">Optional color to use (defaults to warm white)</param>
        /// <returns>GUID of the created override</returns>
        Task<string> AddOverride(DateTime endTime, AlarmClockColor color = null);

        /// <summary>
        /// Remove a specific override by GUID
        /// </summary>
        /// <param name="guid">The GUID of the override to remove</param>
        /// <returns>True if an override was removed, false if not found</returns>
        Task<bool> RemoveOverride(string guid);

        /// <summary>
        /// Clear all active overrides
        /// </summary>
        Task ClearAllOverrides();

        /// <summary>
        /// Get the count of active overrides
        /// </summary>
        Task<int> GetOverrideCount();

        Task<AlarmClockColor> GetCurrentColor();
        Task<bool> IsLightCurrentlyOn();
        DateTime GetScaledTime();
        Task<IConfigurableColorPickingServiceParameters> GetCurrentParameters();
    }
}