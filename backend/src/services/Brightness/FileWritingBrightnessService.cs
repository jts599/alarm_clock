using System;
using System.IO;

namespace Backend.Services.Brightness
{
    public abstract class FileWritingBrightnessService : IBrightnessService
    {
        protected abstract string BrightnessFilePath { get; }
        protected abstract string MaxBrightnessFilePath { get; }

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
