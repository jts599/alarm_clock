using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AlarmClock.Backend.Configuration;
using AlarmClock.Backend.DataModels.Weather;
using Microsoft.AspNetCore.Http;

namespace AlarmClock.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {

        private readonly IOptions<WeatherConfiguration> _weatherConfig;
        public WeatherController(IOptions<WeatherConfiguration> weatherConfig)
        {
            _weatherConfig = weatherConfig;
        }



        /// <summary>
        /// Retrieves the configured weather location coordinates.
        /// </summary>
        /// <remarks>
        /// Returns the longitude and latitude values from the application's weather configuration.
        /// This endpoint does not require any input parameters and returns the statically configured location.
        /// </remarks>
        /// <returns>A <see cref="WeatherLocationResponse"/> containing the longitude and latitude coordinates.</returns>
        /// <response code="200">Returns the configured weather location coordinates successfully.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [ProducesResponseType(typeof(WeatherLocationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("location")]
        public ActionResult<WeatherLocationResponse> GetLocation()
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