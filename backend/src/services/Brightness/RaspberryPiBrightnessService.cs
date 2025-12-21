using System;
using System.IO;
using System.Linq;

namespace AlarmClock.Backend.Services.Brightness
{
    /// <summary>
    /// Concrete implementation of brightness control for Raspberry Pi hardware.
    /// Automatically discovers and uses the first available backlight device in /sys/class/backlight.
    /// </summary>
    /// <remarks>
    /// This service works with various Raspberry Pi displays including:
    /// - Official 7" touchscreen (rpi_backlight)
    /// - Third-party displays with backlight control
    /// The actual device name is discovered at runtime, making this compatible with different hardware.
    /// </remarks>
    public class RaspberryPiBrightnessService : FileWritingBrightnessService
    {
        /// <summary>
        /// The full path to the discovered backlight device directory.
        /// </summary>
        private readonly string _backlightPath;

        /// <summary>
        /// Initializes a new instance of the RaspberryPiBrightnessService class.
        /// Automatically discovers the first available backlight device.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when /sys/class/backlight does not exist or contains no devices.</exception>
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

        /// <summary>
        /// Gets the path to the brightness control file for the discovered backlight device.
        /// </summary>
        /// <value>The full path to the brightness file (e.g., /sys/class/backlight/rpi_backlight/brightness).</value>
        protected override string BrightnessFilePath => Path.Combine(_backlightPath, "brightness");

        /// <summary>
        /// Gets the path to the maximum brightness file for the discovered backlight device.
        /// </summary>
        /// <value>The full path to the max_brightness file (e.g., /sys/class/backlight/rpi_backlight/max_brightness).</value>
        protected override string MaxBrightnessFilePath => Path.Combine(_backlightPath, "max_brightness");
    }
}
