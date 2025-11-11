using System;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        Task SwapColorPicker(ICompositeColorPickingService newColorPicker);
        Task SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker);
        Task<ICompositeColorPickingService> GetCurrentColorPickerCopy();

        Task<AlarmClockColor> GetCurrentColor();
        Task<bool> IsLightCurrentlyOn();
        DateTime GetScaledTime();
        Task<IConfigurableColorPickingServiceParameters> GetCurrentParameters();
    }
}