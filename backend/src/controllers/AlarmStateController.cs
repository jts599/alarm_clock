using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlarmStateController : ControllerBase
{
    private readonly ILifxService _lifxService;
    private readonly ILightStateService _lightStateService;

    public AlarmStateController(ILifxService lifxService, ILightStateService lightStateService)
    {
        _lifxService = lifxService;
        _lightStateService = lightStateService;
    }

    [HttpGet("number-of-active-bulbs")]
    public async Task<ActionResult<int>> GetNumberOfActiveBulbs()
    {
        var activeBulbs = await _lifxService.GetNumberOfBulbsAsync();
        return Ok(activeBulbs);
    }

    [HttpGet("next-event")]
    public Task<ActionResult<AlarmEventInfo>> GetNextAlarmEventInfo()
    {
        // TODO: Implement actual logic to get next alarm event info
        return Task.FromResult<ActionResult<AlarmEventInfo>>(Ok(new AlarmEventInfo
        {
            NextEventDayOfWeek = "Monday",
            NextEventTime = "07:00 AM",
            NextEventType = EventType.Sunrise,
            IsAlarmActive = true
        }));
    }

    [HttpPost("turn-on-until")]
    public async Task<ActionResult<CreateLightOverrideResponse>> TurnLightOnUntil([FromBody] CreateLightOverrideRequest request)
    {
        var colorOverride = new LightOnOverrideColorPickingService(request.EndTime);
        var colorPicker = await _lightStateService.GetCurrentColorPickerCopy() as OverrideableColorPickingService;
        if (colorPicker == null)
        {
            return BadRequest("Current color picker does not support overrides.");
        }
        colorPicker.AddOverride(colorOverride);
        await _lightStateService.SwapColorPicker(colorPicker);

        return Ok(new CreateLightOverrideResponse
        {
            OverrideGuid = colorOverride.guid
        });
    }

    [HttpDelete("remove-override")]
    public async Task<ActionResult> RemoveAlarmOverride([FromBody] RemoveAlarmOverrideRequest request)
    {
        var colorPicker = await _lightStateService.GetCurrentColorPickerCopy() as OverrideableColorPickingService;
        if (colorPicker == null)
        {
            return NotFound();
        }
        int beforeCount = colorPicker.GetOverrideCount();
        colorPicker?.ClearOverrideByGuid(request.OverrideGuid);
        int afterCount = colorPicker.GetOverrideCount();
        await _lightStateService.SwapColorPicker(colorPicker);
        if (beforeCount > afterCount)
        {
            return Ok();
        }
        return NotFound();
    }
}

public class AlarmEventInfo
{
    public string NextEventDayOfWeek { get; set; } = string.Empty;
    public string NextEventTime { get; set; } = string.Empty;
    public EventType NextEventType { get; set; }
    public bool IsAlarmActive { get; set; }
}

public enum EventType
{
    LightOff = 0,
    Sunrise = 1
}

public class TurnLightOnRequest
{
    public DateTime NextEventTime { get; set; }
}

public class RemoveAlarmOverrideRequest
{
    public string OverrideGuid { get; set; } = string.Empty;
}

public class CreateLightOverrideRequest
{
    public DateTime EndTime { get; set; }
}

public class CreateLightOverrideResponse
{
    public string OverrideGuid { get; set; } = string.Empty;
}
