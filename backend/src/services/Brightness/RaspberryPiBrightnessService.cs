using System;
using System.IO;
using System.Linq;

namespace Backend.Services.Brightness
{
    public class RaspberryPiBrightnessService : FileWritingBrightnessService
    {
        private readonly string _backlightPath;

        public RaspberryPiBrightnessService()
        {
            // Find the first available backlight device
            const string backlightBaseDir = "/sys/class/backlight";
            
            if (!Directory.Exists(backlightBaseDir))
            {
                throw new DirectoryNotFoundException($"Backlight directory not found: {backlightBaseDir}");
            }

            var backlightDirs = Directory.GetDirectories(backlightBaseDir);
            if (backlightDirs.Length == 0)
            {
                throw new DirectoryNotFoundException("No backlight devices found in /sys/class/backlight");
            }

            // Use the first available backlight device
            // Common names: rpi_backlight, 10-0045, etc.
            _backlightPath = backlightDirs.First();
        }

        protected override string BrightnessFilePath => Path.Combine(_backlightPath, "brightness");
        protected override string MaxBrightnessFilePath => Path.Combine(_backlightPath, "max_brightness");
    }
}
