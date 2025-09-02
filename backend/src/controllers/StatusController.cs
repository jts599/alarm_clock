using Microsoft.AspNetCore.Mvc;
using System;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "UP",
                service = "Alarm Clock Backend",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
