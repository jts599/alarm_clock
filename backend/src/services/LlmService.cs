using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Tokenizers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using AlarmClock.Backend.Configuration;

namespace AlarmClock.Backend.Services
{
    public class LlmService : ILlmService, IDisposable
    {
        private readonly ILogger<LlmService> _logger;
        private readonly LlmConfiguration _config;
        private InferenceSession _session;
        private Tokenizer _tokenizer;
        private readonly object _lock = new object();
        private bool _isInitialized = false;

        // Valid weather icons from your shared file
        private readonly string[] _validIconsList = new[]
        {
            "weather.day.cloudy-gusts", "weather.day.cloudy", "weather.day.fog", "weather.day.hail",
            "weather.day.haze", "weather.day.lightning", "weather.day.rain-mix", "weather.day.rain",
            "weather.day.showers", "weather.day.sleet-storm", "weather.day.sleet", "weather.day.snow-thunderstorm",
            "weather.day.snow-wind", "weather.day.snow", "weather.day.sprinkle", "weather.day.storm-showers",
            "weather.day.sunny-overcast", "weather.day.sunny", "weather.day.thunderstorm", "weather.day.windy",
            "weather.night.cloudy-gusts", "weather.night.cloudy", "weather.night.hail", "weather.night.lightning",
            "weather.night.partly-cloudy", "weather.night.rain-mix", "weather.night.rain-wind", "weather.night.rain",
            "weather.night.showers", "weather.night.sleet-storm", "weather.night.sleet", "weather.night.snow-thunderstorm",
            "weather.night.snow", "weather.night.sprinkle", "weather.night.stars", "weather.night.storm-showers",
            "weather.night.thunderstorm", "weather.cloud", "weather.cloudy-gusts", "weather.cloudy",
            "weather.dust", "weather.fog", "weather.hail", "weather.horizon", "weather.hot",
            "weather.hurricane", "weather.lightning", "weather.na", "weather.rain-mix", "weather.rain-wind",
            "weather.rain", "weather.raindrops", "weather.showers", "weather.sleet", "weather.snow-wind",
            "weather.snow", "weather.snowflake-cold", "weather.sprinkle", "weather.storm-showers", "weather.thunderstorm"
        };

        private readonly Dictionary<string, string[]> _weatherKeywords = new()
        {
            { "sunny", new[] { "weather.day.sunny", "weather.night.stars" } },
            { "clear", new[] { "weather.day.sunny", "weather.night.stars" } },
            { "rain", new[] { "weather.day.rain", "weather.night.rain" } },
            { "heavy rain", new[] { "weather.day.rain", "weather.night.rain" } },
            { "light rain", new[] { "weather.day.showers", "weather.night.showers" } },
            { "drizzle", new[] { "weather.day.sprinkle", "weather.night.sprinkle" } },
            { "thunderstorm", new[] { "weather.day.thunderstorm", "weather.night.thunderstorm" } },
            { "thunder", new[] { "weather.day.lightning", "weather.night.lightning" } },
            { "storm", new[] { "weather.day.storm-showers", "weather.night.storm-showers" } },
            { "snow", new[] { "weather.day.snow", "weather.night.snow" } },
            { "sleet", new[] { "weather.day.sleet", "weather.night.sleet" } },
            { "hail", new[] { "weather.day.hail", "weather.night.hail" } },
            { "cloud", new[] { "weather.day.cloudy", "weather.night.cloudy" } },
            { "cloudy", new[] { "weather.day.cloudy", "weather.night.cloudy" } },
            { "partly cloudy", new[] { "weather.day.sunny-overcast", "weather.night.partly-cloudy" } },
            { "overcast", new[] { "weather.cloudy", "weather.cloudy" } },
            { "fog", new[] { "weather.day.fog", "weather.fog" } },
            { "foggy", new[] { "weather.day.fog", "weather.fog" } },
            { "mist", new[] { "weather.day.fog", "weather.fog" } },
            { "wind", new[] { "weather.day.windy", "weather.night.cloudy-gusts" } },
            { "windy", new[] { "weather.day.windy", "weather.night.cloudy-gusts" } },
            { "hot", new[] { "weather.hot", "weather.day.sunny" } },
            { "cold", new[] { "weather.snowflake-cold", "weather.snowflake-cold" } }
        };

        public LlmService(IOptions<LlmConfiguration> config, ILogger<LlmService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        private async Task InitializeAsync()
        {
            if (_isInitialized || !_config.EnableLlm)
                return;

            lock (_lock)
            {
                if (_isInitialized)
                    return;

                try
                {
                    if (string.IsNullOrEmpty(_config.ModelPath) || !File.Exists(_config.ModelPath))
                    {
                        _logger.LogWarning("ONNX model path not found: {ModelPath}. Using fallback icon selection.", _config.ModelPath);
                        return;
                    }

                    var sessionOptions = new SessionOptions
                    {
                        ExecutionMode = ExecutionMode.ORT_SEQUENTIAL,
                        EnableProfiling = false,
                        InterOpNumThreads = 1,  // Good for ARM
                        IntraOpNumThreads = 2   // Adjust based on ARM cores
                    };

                    _session = new InferenceSession(_config.ModelPath, sessionOptions);

                    // We'll initialize tokenizer only when needed for specific models
                    _tokenizer = null;

                    _isInitialized = true;
                    _logger.LogInformation("ONNX LLM initialized successfully with model: {ModelPath}", _config.ModelPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize ONNX LLM");
                }
            }
        }

        public async Task<string> SelectWeatherIconAsync(string weatherDescription)
        {
            if (!_config.EnableLlm)
            {
                _logger.LogInformation("LLM disabled, using enhanced fallback for: {Weather}", weatherDescription);
                return SelectEnhancedFallbackIcon(weatherDescription);
            }

            await InitializeAsync();

            if (!_isInitialized || _session == null)
            {
                _logger.LogInformation("LLM not available, using enhanced fallback for: {Weather}", weatherDescription);
                return SelectEnhancedFallbackIcon(weatherDescription);
            }

            // For now, we'll use the enhanced fallback since ONNX text generation requires specific model setup
            // This can be enhanced later with a proper ONNX text generation model
            _logger.LogInformation("Using enhanced fallback (ONNX setup requires specific model): {Weather}", weatherDescription);
            return SelectEnhancedFallbackIcon(weatherDescription);
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            await InitializeAsync();

            if (!_isInitialized || _session == null)
            {
                throw new InvalidOperationException("ONNX LLM is not initialized");
            }

            // This would require specific ONNX model implementation
            // For now, return a placeholder
            return "ONNX response placeholder";
        }

        private string SelectEnhancedFallbackIcon(string weatherDescription)
        {
            var desc = weatherDescription.ToLowerInvariant();
            var hour = DateTime.Now.Hour;
            var isDay = hour >= 6 && hour < 19;

            _logger.LogDebug("Selecting icon for '{Description}', isDay: {IsDay}", desc, isDay);

            // Score-based matching for better accuracy
            var scores = new Dictionary<string, int>();

            foreach (var keyword in _weatherKeywords.Keys)
            {
                if (desc.Contains(keyword))
                {
                    var icons = _weatherKeywords[keyword];
                    var selectedIcon = isDay ? icons[0] : icons[1];

                    // Weight longer/more specific matches higher
                    var score = keyword.Length;
                    if (keyword.Contains(" ")) score += 10; // Multi-word phrases get bonus

                    if (scores.ContainsKey(selectedIcon))
                        scores[selectedIcon] += score;
                    else
                        scores[selectedIcon] = score;
                }
            }

            if (scores.Any())
            {
                var bestIcon = scores.OrderByDescending(kvp => kvp.Value).First().Key;
                _logger.LogInformation("Selected icon '{Icon}' with score {Score} for '{Description}'",
                    bestIcon, scores[bestIcon], weatherDescription);
                return bestIcon;
            }

            // Ultimate fallback
            var fallback = isDay ? "weather.day.sunny" : "weather.night.stars";
            _logger.LogInformation("No matches found, using fallback icon '{Icon}' for '{Description}'",
                fallback, weatherDescription);
            return fallback;
        }

        public void Dispose()
        {
            _session?.Dispose();
            // Tokenizer disposal will be handled when we implement specific tokenizers
        }
    }
}