using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BackgroundController : ControllerBase
    {
        // Try multiple locations: Docker production path, then relative to project root
        private static readonly string[] CustomBackgroundPaths = new[]
        {
            "/app/background-custom.jpg",                    // Docker production
            "../background-custom.jpg",                      // Local development (relative to backend/)
            Path.Combine(Directory.GetCurrentDirectory(), "..", "background-custom.jpg") // Absolute from current dir
        };

        private string? GetCustomBackgroundPath()
        {
            foreach (var path in CustomBackgroundPaths)
            {
                var resolvedPath = Path.GetFullPath(path);
                if (System.IO.File.Exists(resolvedPath))
                {
                    return resolvedPath;
                }
            }
            return null;
        }

        [HttpGet("custom")]
        public IActionResult GetCustomBackground()
        {
            var customBackgroundPath = GetCustomBackgroundPath();
            if (customBackgroundPath == null)
            {
                return NotFound(new { message = "Custom background not configured" });
            }

            try
            {
                var fileBytes = System.IO.File.ReadAllBytes(customBackgroundPath);
                return File(fileBytes, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reading custom background", error = ex.Message });
            }
        }

        [HttpGet("custom/exists")]
        public IActionResult CheckCustomBackgroundExists()
        {
            var customBackgroundPath = GetCustomBackgroundPath();
            var exists = customBackgroundPath != null;
            return Ok(new { exists, path = exists ? "/api/background/custom" : null });
        }
    }
}
