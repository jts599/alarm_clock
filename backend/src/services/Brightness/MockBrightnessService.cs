using Microsoft.Extensions.Logging;

namespace Backend.Services.Brightness
{
    public class MockBrightnessService : IBrightnessService
    {
        private readonly ILogger<MockBrightnessService> _logger;
        private int _currentBrightness = 128;
        private const int _maxBrightness = 255;

        public MockBrightnessService(ILogger<MockBrightnessService> logger)
        {
            _logger = logger;
            _logger.LogInformation("MockBrightnessService initialized - using mock brightness implementation");
        }

        public int GetBrightness()
        {
            _logger.LogDebug("MockBrightnessService: GetBrightness called, returning {Brightness}", _currentBrightness);
            return _currentBrightness;
        }

        public int GetMaxBrightness()
        {
            _logger.LogDebug("MockBrightnessService: GetMaxBrightness called, returning {MaxBrightness}", _maxBrightness);
            return _maxBrightness;
        }

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
