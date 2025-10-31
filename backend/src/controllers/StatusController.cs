using Microsoft.AspNetCore.Mvc;
using System;
using AlarmClock.Backend.Services;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILightStateService _lightStateService;

        public StatusController(ILightStateService lightStateService)
        {
            _lightStateService = lightStateService;
        }
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

        [HttpGet("color")]
        public IActionResult GetColorSimulation()
        {
            // Get the current color from the LightStateService
            var currentColor = _lightStateService.GetCurrentColor();
            int[] rgb = ColorUtils.GetRgbFromTemperature(currentColor.Kelvin);

            int scaledRed = (int)(rgb[0] * currentColor.Color.R / 255.0);
            int scaledGreen = (int)(rgb[1] * currentColor.Color.G / 255.0);
            int scaledBlue = (int)(rgb[2] * currentColor.Color.B / 255.0);

            return Ok(new
            {
                red = scaledRed,
                green = scaledGreen,
                blue = scaledBlue,
                currentTimeString = _lightStateService.GetScaledTime().ToString("HH:mm")
            });
        }


        [HttpGet("live")]
        public IActionResult GetBrowserPageWithBackgroundSimulatingBulb()
        {
            var htmlContent = @"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Live Bulb Simulation</title>
                    <style>
                        body, html {
                            height: 100%;
                            margin: 0;
                            display: flex;
                            justify-content: center;
                            align-items: center;
                            transition: background-color 1s;
                        }
                        #status {
                            font-size: 2em;
                            color: white;
                            text-shadow: 0 0 5px black;
                        }
                    </style>
                </head>
                <body>
                    <div id='status'>Simulating Bulb Color...</div>
                    <script>
                        async function fetchBulbColor() {
                            try {
                                const response = await fetch('/api/status/color');
                                const data = await response.json();
                                document.body.style.backgroundColor = `rgb(${data.red}, ${data.green}, ${data.blue})`;
                                document.getElementById('status').innerText = `Current Time: ${data.currentTimeString} | Color RGB(${data.red}, ${data.green}, ${data.blue})`;
                            } catch (error) {
                                console.error('Error fetching bulb color:', error);
                            }
                        }
                        setInterval(fetchBulbColor, 2000);
                        fetchBulbColor();
                    </script>
                </body>
                </html>";
            return Content(htmlContent, "text/html");
        }
    }


    //Ripped from:
    //https://tannerhelland.com/2012/09/18/convert-temperature-rgb-algorithm-code.html
    public class ColorUtils
    {
        public static int[] GetRgbFromTemperature(double temperature)
        {
            // Temperature must fit between 1000 and 40000 degrees.
            temperature = Math.Clamp(temperature, 1000, 40000);

            // All calculations require temperature / 100, so only do the conversion once.
            temperature /= 100;

            // Compute each color in turn.
            int red, green, blue;

            // First: red.
            if (temperature <= 66)
            {
                red = 255;
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.988.
                red = (int)(329.698727446 * (Math.Pow(temperature - 60, -0.1332047592)));
                red = Math.Clamp(red, 0, 255);
            }

            // Second: green.
            if (temperature <= 66)
            {
                // Note: the R-squared value for this approximation is 0.996.
                green = (int)(99.4708025861 * Math.Log(temperature) - 161.1195681661);
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.987.
                green = (int)(288.1221695283 * (Math.Pow(temperature - 60, -0.0755148492)));
            }

            green = Math.Clamp(green, 0, 255);

            // Third: blue.
            if (temperature >= 66)
            {
                blue = 255;
            }
            else if (temperature <= 19)
            {
                blue = 0;
            }
            else
            {
                // Note: the R-squared value for this approximation is 0.998.
                blue = (int)(138.5177312231 * Math.Log(temperature - 10) - 305.0447927307);
                blue = Math.Clamp(blue, 0, 255);
            }

            return new[] { red, green, blue };
        }
    }
}