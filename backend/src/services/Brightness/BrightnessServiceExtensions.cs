using System;

namespace AlarmClock.Backend.Services.Brightness
{
    /// <summary>
    /// Extension methods for IBrightnessService that provide normalized brightness control (0-100%).
    /// Converts between hardware-specific brightness values and percentage-based values.
    /// </summary>
    public static class BrightnessServiceExtensions
    {
        /// <summary>
        /// Gets the current brightness as a normalized percentage (0-100).
        /// </summary>
        /// <param name="service">The brightness service instance.</param>
        /// <returns>The current brightness as a percentage (0-100).</returns>
        /// <exception cref="InvalidOperationException">Thrown when brightness cannot be read.</exception>
        /// <remarks>
        /// Calculates the percentage by dividing the current brightness by the maximum brightness
        /// and multiplying by 100. The result is rounded to the nearest integer.
        /// </remarks>
        public static int GetBrightnessPercent(this IBrightnessService service)
        {
            int current = service.GetBrightness();
            int max = service.GetMaxBrightness();

            if (max == 0)
            {
                return 0;
            }

            return (int)Math.Round((double)current / max * 100);
        }

        /// <summary>
        /// Sets the brightness using a normalized percentage (0-100).
        /// Converts the percentage to the appropriate hardware-specific value.
        /// </summary>
        /// <param name="service">The brightness service instance.</param>
        /// <param name="percent">The brightness percentage to set (0-100).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when percent is less than 0 or greater than 100.</exception>
        /// <exception cref="InvalidOperationException">Thrown when brightness cannot be set.</exception>
        /// <remarks>
        /// Calculates the hardware-specific brightness value by multiplying the percentage
        /// by the maximum brightness and dividing by 100. The result is rounded to the nearest integer.
        /// A value of 0% sets brightness to 0, and 100% sets it to the maximum brightness.
        /// </remarks>
        public static void SetBrightnessPercent(this IBrightnessService service, int percent)
        {
            if (percent < 0 || percent > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(percent),
                    $"Brightness percent must be between 0 and 100, but was {percent}");
            }

            int max = service.GetMaxBrightness();
            int brightness = (int)Math.Round((double)percent / 100 * max);

            service.SetBrightness(brightness);
        }
    }
}
