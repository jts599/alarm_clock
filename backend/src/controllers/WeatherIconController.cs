using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AlarmClock.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherIconController : ControllerBase
    {
        private readonly ILlmService _llmService;

        public WeatherIconController(ILlmService llmService)
        {
            _llmService = llmService;
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectIcon([FromBody] WeatherIconRequest request)
        {
            if (string.IsNullOrEmpty(request.WeatherDescription))
            {
                return BadRequest("Weather description is required");
            }

            var icon = await _llmService.SelectWeatherIconAsync(request.WeatherDescription);

            return Ok(new WeatherIconResponse
            {
                Icon = icon,
                WeatherDescription = request.WeatherDescription
            });
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var testCases = new[]
            {
                "Sunny and clear skies",
                "Heavy rain with thunderstorms",
                "Light snow falling",
                "Partly cloudy",
                "Foggy morning"
            };

            var results = new List<object>();

            foreach (var testCase in testCases)
            {
                var icon = await _llmService.SelectWeatherIconAsync(testCase);
                results.Add(new { weatherDescription = testCase, selectedIcon = icon });
            }

            return Ok(results);
        }
    }

    public class WeatherIconRequest
    {
        public string WeatherDescription { get; set; } = "";
    }

    public class WeatherIconResponse
    {
        public string Icon { get; set; } = "";
        public string WeatherDescription { get; set; } = "";
    }
}