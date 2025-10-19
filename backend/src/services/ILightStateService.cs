namespace MyFullstackApp.Services
{
    public interface ILightStateService
    {
        void SwapColorPicker(IColorPickingService newColorPicker);
    }
}