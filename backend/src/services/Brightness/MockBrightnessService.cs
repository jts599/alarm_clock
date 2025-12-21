using System;
using Microsoft.Extensions.Logging;

namespace Backend.Services.Brightness
{
    /// <summary>
    /// Mock implementation of IBrightnessService for development and testing.
    /// Stores brightness values in memory and logs all operations without interacting with hardware.
    /// </summary>
    /// <remarks>
    /// This service is useful for:
    /// - Development environments without physical hardware
    /// - Testing brightness control logic
    /// - Running in containers or environments without /sys/class/backlight access
    /// Configure via RunConfiguration.StubBrightness in appsettings.
    /// </remarks>
    public class MockBrightnessService : IBrightnessService
    {
        private readonly ILogger<MockBrightnessService> _logger;

        /// <summary>
        /// The current brightness level stored in memory.
        /// </summary>
        private int _currentBrightness = 128;

        /// <summary>
        /// The maximum brightness level supported by this mock service.
        /// </summary>
        private const int _maxBrightness = 255;

        /// <summary>
        /// Initializes a new instance of the MockBrightnessService class.
        /// </summary>
        /// <param name="logger">Logger for recording brightness operations.</param>
        public MockBrightnessService(ILogger<MockBrightnessService> logger)
        {
            _logger = logger;
            _logger.LogInformation("MockBrightnessService initialized - using mock brightness implementation");
        }

        /// <summary>
        /// Gets the current mock brightness value stored in memory.
        /// </summary>
        /// <returns>The current brightness level (0-255).</returns>
        public int GetBrightness()
        {
            _logger.LogDebug("MockBrightnessService: GetBrightness called, returning {Brightness}", _currentBrightness);
            return _currentBrightness;
        }

        /// <summary>
        /// Gets the maximum brightness value supported by this mock service.
        /// </summary>
        /// <returns>The maximum brightness level (255).</returns>
        public int GetMaxBrightness()
        {
            _logger.LogDebug("MockBrightnessService: GetMaxBrightness called, returning {MaxBrightness}", _maxBrightness);
            return _maxBrightness;
        }

        /// <summary>
        /// Sets the mock brightness value in memory.
        /// Validates that the brightness is within the valid range.
        /// </summary>
        /// <param name="brightness">The brightness value to set (0-255).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when brightness is negative or exceeds 255.</exception>
        public void SetBrightness(int brightness)
        {
            if (brightness < 0 || brightness > _maxBrightness)
            {
                _logger.LogWarning("MockBrightnessService: Invalid brightness value {Brightness}, must be between 0 and {MaxBrightness}",
                    brightness, _maxBrightness);
                throw new ArgumentOutOfRangeException(nameof(brightness),
                    $"Brightness must be between 0 and {_maxBrightness}");
            }

            _logger.LogInformation("MockBrightnessService: Setting brightness to {Brightness}", brightness);
            _currentBrightness = brightness;
        }
    }
}
