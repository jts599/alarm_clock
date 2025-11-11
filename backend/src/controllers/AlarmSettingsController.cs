using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlarmClock.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlarmSettingsController : ControllerBase
    {
        private readonly ILightStateService _lightStateService;

        public AlarmSettingsController(ILightStateService lightStateService)
        {
            _lightStateService = lightStateService;
        }

        [HttpGet]
        public async Task<ActionResult<UserSettingsDto>> GetUserSettings()
        {
            try
            {
                var currentParameters = await _lightStateService.GetCurrentParameters();

                var settings = new UserSettingsDto
                {
                    AlarmTimeInMinutesSinceMidnight = currentParameters.AlarmTime.Hour * 60 + currentParameters.AlarmTime.Minute,
                    TransitionMinutes = currentParameters.TransitionMinutes,
                    TurnOffAfterMinutes = currentParameters.HoldOnMinutes,
                    EnabledDaysOfWeek = currentParameters.ActiveDays.Select(d => d.ToString()).ToArray()
                };

                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user settings: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserSettings([FromBody] UserSettingsDto settings)
        {
            try
            {
                if (settings == null)
                {
                    return BadRequest("Settings cannot be null");
                }

                // Convert minutes since midnight to TimeOnly
                var hours = settings.AlarmTimeInMinutesSinceMidnight / 60;
                var minutes = settings.AlarmTimeInMinutesSinceMidnight % 60;
                var alarmTime = new TimeOnly(hours, minutes);

                // Parse days of week
                var activeDays = settings.EnabledDaysOfWeek
                    .Select(day => Enum.Parse<DayOfWeek>(day, true))
                    .ToArray();

                // Create new parameters - this will validate using the built-in validation
                var newParameters = new ConfigurableColorPickingServiceConstructionParameters
                {
                    AlarmTime = alarmTime,
                    TransitionMinutes = settings.TransitionMinutes,
                    HoldOnMinutes = settings.TurnOffAfterMinutes,
                    ActiveDays = activeDays
                };

                // Create new color picking service with updated parameters
                var newColorPickingService = new ConfigurableColorPickingService(newParameters);

                // Swap the base color picker in the light state service
                await _lightStateService.SwapBaseColorPicker(newColorPickingService);

                return Ok(new { message = "Settings updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Invalid settings: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user settings: {ex.Message}");
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<AlarmStatusDto>> GetAlarmStatus()
        {
            try
            {
                var currentTime = _lightStateService.GetScaledTime();
                var currentParameters = await _lightStateService.GetCurrentParameters();
                var isLightOn = await _lightStateService.IsLightCurrentlyOn();
                var currentColor = await _lightStateService.GetCurrentColor();

                var status = new AlarmStatusDto
                {
                    CurrentTime = currentTime,
                    IsLightCurrentlyOn = isLightOn,
                    CurrentColor = new ColorDto
                    {
                        R = currentColor.Color.R,
                        G = currentColor.Color.G,
                        B = currentColor.Color.B,
                        Kelvin = currentColor.Kelvin
                    },
                    NextAlarmTime = GetNextAlarmTime(currentParameters),
                    AlarmTime = currentParameters.AlarmTime,
                    TransitionMinutes = currentParameters.TransitionMinutes,
                    HoldOnMinutes = currentParameters.HoldOnMinutes,
                    ActiveDays = currentParameters.ActiveDays.Select(d => d.ToString()).ToArray()
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving alarm status: {ex.Message}");
            }
        }

        private DateTime GetNextAlarmTime(IConfigurableColorPickingServiceParameters parameters)
        {
            var today = DateTime.Today;
            var alarmTimeToday = today.Add(parameters.AlarmTime.ToTimeSpan());

            // If today's alarm time has passed or today is not an active day, find the next active day
            if (alarmTimeToday <= DateTime.Now || !parameters.ActiveDays.Contains(DateTime.Now.DayOfWeek))
            {
                for (int i = 1; i <= 7; i++)
                {
                    var checkDate = today.AddDays(i);
                    if (parameters.ActiveDays.Contains(checkDate.DayOfWeek))
                    {
                        return checkDate.Add(parameters.AlarmTime.ToTimeSpan());
                    }
                }
            }

            return alarmTimeToday;
        }
    }

    // DTOs matching the frontend interface
    public class UserSettingsDto
    {
        public int AlarmTimeInMinutesSinceMidnight { get; set; }
        public int TransitionMinutes { get; set; }
        public int TurnOffAfterMinutes { get; set; }
        public string[] EnabledDaysOfWeek { get; set; }
    }

    public class AlarmStatusDto
    {
        public DateTime CurrentTime { get; set; }
        public bool IsLightCurrentlyOn { get; set; }
        public ColorDto CurrentColor { get; set; }
        public DateTime NextAlarmTime { get; set; }
        public TimeOnly AlarmTime { get; set; }
        public int TransitionMinutes { get; set; }
        public int HoldOnMinutes { get; set; }
        public string[] ActiveDays { get; set; }
    }

    public class ColorDto
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public ushort Kelvin { get; set; }
    }
}