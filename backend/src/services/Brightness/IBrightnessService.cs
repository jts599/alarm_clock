namespace Backend.Services.Brightness
{
    public interface IBrightnessService
    {
        int GetBrightness();
        int GetMaxBrightness();
        void SetBrightness(int brightness);
    }
}