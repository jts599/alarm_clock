using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services.Brightness;
using System;

namespace Backend.Controllers
{
    /// <summary>
    /// Controller for managing screen brightness on the Raspberry Pi.
    /// Provides endpoints to get and set brightness using percentage values (0-100).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BrightnessController : ControllerBase
    {
        private readonly IBrightnessService _brightnessService;

        /// <summary>
        /// Initializes a new instance of the BrightnessController class.
        /// </summary>
        /// <param name="brightnessService">The brightness service for hardware control.</param>
        public BrightnessController(IBrightnessService brightnessService)
        {
            _brightnessService = brightnessService;
        }

        /// <summary>
        /// Gets the current screen brightness as a percentage (0-100).
        /// </summary>
        /// <returns>An object containing the current brightness percentage.</returns>
        /// <response code="200">Returns the current brightness percentage.</response>
        /// <response code="500">If an error occurs reading the brightness.</response>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult GetBrightness()
        {
            try
            {
                int percent = _brightnessService.GetBrightnessPercent();
                return Ok(new
                {
                    brightness = percent,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to read brightness",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Sets the screen brightness using a percentage value (0-100).
        /// </summary>
        /// <param name="request">Request object containing the brightness percentage.</param>
        /// <returns>Confirmation of the brightness change.</returns>
        /// <response code="200">Brightness was successfully set.</response>
        /// <response code="400">The brightness value is invalid (not between 0-100).</response>
        /// <response code="500">If an error occurs setting the brightness.</response>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult SetBrightness([FromBody] SetBrightnessRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { error = "Request body is required" });
            }

            try
            {
                _brightnessService.SetBrightnessPercent(request.Brightness);
                return Ok(new
                {
                    brightness = request.Brightness,
                    message = $"Brightness set to {request.Brightness}%",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new
                {
                    error = "Invalid brightness value",
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to set brightness",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Gets the maximum brightness value supported by the hardware.
        /// </summary>
        /// <returns>An object containing the maximum brightness value.</returns>
        /// <response code="200">Returns the maximum brightness value.</response>
        /// <response code="500">If an error occurs reading the max brightness.</response>
        [HttpGet("max")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public IActionResult GetMaxBrightness()
        {
            try
            {
                int max = _brightnessService.GetMaxBrightness();
                return Ok(new
                {
                    maxBrightness = max,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to read max brightness",
                    message = ex.Message
                });
            }
        }
    }

    /// <summary>
    /// Request model for setting brightness.
    /// </summary>
    public class SetBrightnessRequest
    {
        /// <summary>
        /// The brightness percentage to set (0-100).
        /// </summary>
        public int Brightness { get; set; }
    }
}
