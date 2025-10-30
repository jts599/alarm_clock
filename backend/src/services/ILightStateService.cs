using System;

namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        void SwapColorPicker(IColorPickingService newColorPicker);
        AlarmClockColor GetCurrentColor();
        bool isLightCurrentlyOn();
        DateTime GetScaledTime();
    }
}