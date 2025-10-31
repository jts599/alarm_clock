using System;

namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        void SwapColorPicker(ICompositeColorPickingService newColorPicker);
        void SwapBaseColorPicker(IBaseColorPickingService newBaseColorPicker);
        AlarmClockColor GetCurrentColor();
        bool isLightCurrentlyOn();
        DateTime GetScaledTime();
        IConfigurableColorPickingServiceParameters GetCurrentParameters();
    }
}