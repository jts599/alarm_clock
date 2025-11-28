using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AlarmClock.Backend.Configuration;
using AlarmClock.Backend.DataModels.Weather;

namespace AlarmClock.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppConfigController : ControllerBase
    {

        private readonly IOptions<WeatherConfiguration> _weatherConfig;
        public AppConfigController(IOptions<WeatherConfiguration> weatherConfig)
        {
            _weatherConfig = weatherConfig;
        }

        [HttpPost("location")]
        public async Task<ActionResult<WeatherLocationResponse>> GetLocation()
        {
            var config = _weatherConfig.Value;
            return Ok(new WeatherLocationResponse
            {
                Longitude = config.Longitude,
                Latitude = config.Latitude
            });
        }
    }
}