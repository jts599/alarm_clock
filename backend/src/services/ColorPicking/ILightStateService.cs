using System;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        void SwapColorPicker(ICompositeColorPickingService newColorPicker);
        Task SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker);
        AlarmClockColor GetCurrentColor();
        bool isLightCurrentlyOn();
        DateTime GetScaledTime();
        IConfigurableColorPickingServiceParameters GetCurrentParameters();
    }
}