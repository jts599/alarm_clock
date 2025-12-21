namespace Backend.Services.Brightness
{
    /// <summary>
    /// Interface for controlling screen brightness on the Raspberry Pi.
    /// Provides methods to read and write brightness levels.
    /// </summary>
    public interface IBrightnessService
    {
        /// <summary>
        /// Gets the current brightness level of the screen.
        /// </summary>
        /// <returns>The current brightness value (typically 0 to max brightness).</returns>
        /// <exception cref="InvalidOperationException">Thrown when brightness cannot be read.</exception>
        int GetBrightness();

        /// <summary>
        /// Gets the maximum brightness level supported by the screen.
        /// </summary>
        /// <returns>The maximum brightness value supported by the hardware.</returns>
        /// <exception cref="InvalidOperationException">Thrown when max brightness cannot be read.</exception>
        int GetMaxBrightness();

        /// <summary>
        /// Sets the brightness level of the screen.
        /// </summary>
        /// <param name="brightness">The brightness value to set (must be between 0 and max brightness).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when brightness is negative or exceeds max brightness.</exception>
        /// <exception cref="InvalidOperationException">Thrown when brightness cannot be set.</exception>
        void SetBrightness(int brightness);
    }
}