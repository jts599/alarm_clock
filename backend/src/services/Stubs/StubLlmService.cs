using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AlarmClock.Backend.Services.Stubs
{
    public class StubLlmService : ILlmService
    {
        private readonly ILogger<StubLlmService> _logger;

        public StubLlmService(ILogger<StubLlmService> logger)
        {
            _logger = logger;
        }

        public Task<string> SelectWeatherIconAsync(string weatherDescription)
        {
            _logger.LogInformation("Stub LLM selecting icon for: {Weather}", weatherDescription);

            var desc = weatherDescription.ToLowerInvariant();
            var hour = System.DateTime.Now.Hour;
            var isDay = hour >= 6 && hour < 19;

            // Simple keyword matching
            if (desc.Contains("sunny") || desc.Contains("clear"))
                return Task.FromResult(isDay ? "weather.day.sunny" : "weather.night.stars");
            if (desc.Contains("rain"))
                return Task.FromResult(isDay ? "weather.day.rain" : "weather.night.rain");
            if (desc.Contains("cloud"))
                return Task.FromResult(isDay ? "weather.day.cloudy" : "weather.night.cloudy");
            if (desc.Contains("snow"))
                return Task.FromResult(isDay ? "weather.day.snow" : "weather.night.snow");
            if (desc.Contains("thunder") || desc.Contains("storm"))
                return Task.FromResult(isDay ? "weather.day.thunderstorm" : "weather.night.thunderstorm");

            return Task.FromResult("weather.na");
        }

        public Task<string> GenerateResponseAsync(string prompt)
        {
            _logger.LogInformation("Stub LLM generating response for prompt length: {Length}", prompt.Length);
            return Task.FromResult("This is a stub response from the LLM service.");
        }
    }
}