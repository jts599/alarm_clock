namespace AlarmClock.Backend.Services
{
    public interface ILightStateService
    {
        void SwapColorPicker(IColorPickingService newColorPicker);
    }
}