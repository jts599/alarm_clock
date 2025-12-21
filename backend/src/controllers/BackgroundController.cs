using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

namespace Backend.Controllers
{
    /// <summary>
    /// Controller for serving custom background images.
    /// Supports device-specific backgrounds that are mounted via Docker volumes or placed in the project root.
    /// </summary>
    /// <remarks>
    /// The custom background is expected to be named "background-custom.jpg" and can be placed at:
    /// - /app/background-custom.jpg (Docker production environment)
    /// - ../background-custom.jpg (Local development, relative to backend directory)
    /// 
    /// This allows each device to have a unique background without committing it to source control.
    /// The background is served through the API to avoid build-time detection issues with Vite's static analysis.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class BackgroundController : ControllerBase
    {
        /// <summary>
        /// Ordered list of potential locations for the custom background image.
        /// The first existing file will be used.
        /// </summary>
        private static readonly string[] CustomBackgroundPaths = new[]
        {
            "/app/background-custom.jpg",                    // Docker production
            "../background-custom.jpg",                      // Local development (relative to backend/)
            Path.Combine(Directory.GetCurrentDirectory(), "..", "background-custom.jpg") // Absolute from current dir
        };

        /// <summary>
        /// Searches for the custom background image in known locations.
        /// </summary>
        /// <returns>The full path to the custom background if found; otherwise, an empty string.</returns>
        private string GetCustomBackgroundPath()
        {
            foreach (var path in CustomBackgroundPaths)
            {
                var resolvedPath = Path.GetFullPath(path);
                if (System.IO.File.Exists(resolvedPath))
                {
                    return resolvedPath;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Serves the custom background image as a JPEG file.
        /// </summary>
        /// <returns>
        /// The custom background image file if it exists;
        /// 404 Not Found if no custom background is configured;
        /// 500 Internal Server Error if the file cannot be read.
        /// </returns>
        /// <response code="200">Returns the custom background image as image/jpeg</response>
        /// <response code="404">No custom background is configured</response>
        /// <response code="500">Error reading the custom background file</response>
        [HttpGet("custom")]
        [Produces("image/jpeg", "application/json")]
        public IActionResult GetCustomBackground()
        {
            var customBackgroundPath = GetCustomBackgroundPath();
            if (string.IsNullOrEmpty(customBackgroundPath))
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

        /// <summary>
        /// Checks whether a custom background image is available.
        /// </summary>
        /// <returns>
        /// A JSON object indicating whether a custom background exists and the API path to retrieve it.
        /// </returns>
        /// <response code="200">Returns existence status and path</response>
        [HttpGet("custom/exists")]
        [Produces("application/json")]
        public IActionResult CheckCustomBackgroundExists()
        {
            var customBackgroundPath = GetCustomBackgroundPath();
            var exists = customBackgroundPath != null;
            return Ok(new { exists, path = exists ? "/api/background/custom" : null });
        }
    }
}
