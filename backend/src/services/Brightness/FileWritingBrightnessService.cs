using System;
using System.IO;

namespace Backend.Services.Brightness
{
    /// <summary>
    /// Abstract base class for brightness control via file system writes.
    /// Implements the IBrightnessService interface using file I/O operations
    /// to interact with system brightness controls (typically in /sys/class/backlight).
    /// </summary>
    /// <remarks>
    /// This class provides a standard implementation for reading and writing brightness
    /// values through system files. Concrete implementations must specify the actual
    /// file paths for brightness and max_brightness files.
    /// </remarks>
    public abstract class FileWritingBrightnessService : IBrightnessService
    {
        /// <summary>
        /// Gets the file path to the brightness control file.
        /// Concrete implementations must override this to provide the actual path.
        /// </summary>
        /// <value>The absolute path to the brightness file (e.g., /sys/class/backlight/*/brightness).</value>
        protected abstract string BrightnessFilePath { get; }

        /// <summary>
        /// Gets the file path to the maximum brightness file.
        /// Concrete implementations must override this to provide the actual path.
        /// </summary>
        /// <value>The absolute path to the max brightness file (e.g., /sys/class/backlight/*/max_brightness).</value>
        protected abstract string MaxBrightnessFilePath { get; }

        /// <summary>
        /// Reads the current brightness value from the brightness file.
        /// </summary>
        /// <returns>The current brightness level as an integer.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the brightness file does not exist.</exception>
        /// <exception cref="InvalidDataException">Thrown when the file contains invalid data.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the file cannot be read.</exception>
        public int GetBrightness()
        {
            try
            {
                if (!File.Exists(BrightnessFilePath))
                {
                    throw new FileNotFoundException($"Brightness file not found: {BrightnessFilePath}");
                }

                string content = File.ReadAllText(BrightnessFilePath).Trim();
                if (int.TryParse(content, out int brightness))
                {
                    return brightness;
                }
                throw new InvalidDataException($"Invalid brightness value in file: {content}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read brightness from {BrightnessFilePath}", ex);
            }
        }

        /// <summary>
        /// Reads the maximum brightness value from the max_brightness file.
        /// This value represents the hardware's maximum supported brightness level.
        /// </summary>
        /// <returns>The maximum brightness level supported by the hardware.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the max brightness file does not exist.</exception>
        /// <exception cref="InvalidDataException">Thrown when the file contains invalid data.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the file cannot be read.</exception>
        public int GetMaxBrightness()
        {
            try
            {
                if (!File.Exists(MaxBrightnessFilePath))
                {
                    throw new FileNotFoundException($"Max brightness file not found: {MaxBrightnessFilePath}");
                }

                string content = File.ReadAllText(MaxBrightnessFilePath).Trim();
                if (int.TryParse(content, out int maxBrightness))
                {
                    return maxBrightness;
                }
                throw new InvalidDataException($"Invalid max brightness value in file: {content}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to read max brightness from {MaxBrightnessFilePath}", ex);
            }
        }

        /// <summary>
        /// Writes a new brightness value to the brightness file.
        /// Validates that the brightness is within the valid range (0 to max brightness).
        /// </summary>
        /// <param name="brightness">The brightness value to set. Must be non-negative and not exceed max brightness.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when brightness is negative or exceeds the maximum brightness.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the brightness file does not exist.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the brightness value cannot be written to the file.</exception>
        public void SetBrightness(int brightness)
        {
            try
            {
                int maxBrightness = GetMaxBrightness();

                if (brightness < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(brightness), "Brightness cannot be negative");
                }

                if (brightness > maxBrightness)
                {
                    throw new ArgumentOutOfRangeException(nameof(brightness),
                        $"Brightness {brightness} exceeds maximum {maxBrightness}");
                }

                if (!File.Exists(BrightnessFilePath))
                {
                    throw new FileNotFoundException($"Brightness file not found: {BrightnessFilePath}");
                }

                File.WriteAllText(BrightnessFilePath, brightness.ToString());
            }
            catch (Exception ex) when (ex is not ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException($"Failed to write brightness to {BrightnessFilePath}", ex);
            }
        }
    }
}
