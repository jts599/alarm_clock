using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AlarmClock.Backend.Services;
using AlarmClock.Backend.DataModels.AlarmCore;

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
        string guid = await _lightStateService.AddOverride(request.EndTime, null);
        return Ok(new CreateLightOverrideResponse
        {
            OverrideGuid = guid
        });
    }

    [HttpDelete("remove-override")]
    public async Task<ActionResult> RemoveAlarmOverride([FromBody] RemoveAlarmOverrideRequest request)
    {
        bool success = await _lightStateService.RemoveOverride(request.OverrideGuid);
        if (success)
        {
            return Ok();
        }
        return NotFound();
    }
}
