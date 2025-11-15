using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlarmClock.Backend.Services;
using AlarmClock.Backend.DataModels.AlarmCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlarmStateController : ControllerBase
{
    private readonly ILifxService _lifxService;
    private readonly ILightStateService _lightStateService;

    private readonly IStateSummaryService _stateSummaryService;

    public AlarmStateController(ILifxService lifxService, ILightStateService lightStateService, IStateSummaryService stateSummaryService)
    {
        _lifxService = lifxService;
        _lightStateService = lightStateService;
        _stateSummaryService = stateSummaryService;
    }

    /// <summary>
    /// Get the number of currently reachable/active LIFX bulbs.
    /// </summary>
    /// <remarks>
    /// Uses the underlying LIFX service to count bulbs that are currently responding. Useful for
    /// diagnostics and verifying connectivity to the lighting hardware.
    /// </remarks>
    /// <returns>Integer count of active bulbs.</returns>
    /// <response code="200">Returns the number of active bulbs.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("number-of-active-bulbs")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<int>> GetNumberOfActiveBulbs()
    {
        var activeBulbs = await _lifxService.GetNumberOfBulbsAsync();
        return Ok(activeBulbs);
    }

    /// <summary>
    /// Get information about the next scheduled alarm event.
    /// </summary>
    /// <returns>Details about the next alarm event.</returns>
    /// <response code="200">Returns the next alarm event info.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("next-event")]
    [ProducesResponseType(typeof(AlarmEventInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public Task<ActionResult<AlarmEventInfo>> GetNextAlarmEventInfo()
    {
        // TODO: Implement actual logic to get next alarm event info
        return Task.FromResult<ActionResult<AlarmEventInfo>>(Ok(new AlarmEventInfo
        {
            NextEventDayOfWeek = "Monday",
            NextEventTime = "07:00 AM",
            NextEventType = EventType.Sunrise
        }));
    }

    /// <summary>
    /// Turn the light on until the specified end time by creating a temporary override.
    /// </summary>
    /// <param name="request">Request containing the EndTime for the override.</param>
    /// <returns>Identifier for the created override.</returns>
    /// <response code="200">Override created successfully; returns the override GUID.</response>
    /// <response code="400">Bad request (invalid parameters).</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("turn-on-until")]
    [ProducesResponseType(typeof(CreateLightOverrideResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateLightOverrideResponse>> TurnLightOnUntil([FromBody] CreateLightOverrideRequest request)
    {
        if (request == null)
        {
            return BadRequest("Request cannot be null");
        }

        string guid = await _lightStateService.AddOverride(request.EndTime, null);
        return Ok(new CreateLightOverrideResponse
        {
            OverrideGuid = guid
        });
    }

    /// <summary>
    /// Remove an existing override by its GUID.
    /// </summary>
    /// <param name="request">Request containing the OverrideGuid to remove.</param>
    /// <response code="200">Override removed successfully.</response>
    /// <response code="404">Override not found.</response>
    /// <response code="400">Bad request (invalid parameters).</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("remove-override")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveAlarmOverride([FromBody] RemoveAlarmOverrideRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.OverrideGuid))
        {
            return BadRequest("Invalid override GUID");
        }

        bool success = await _lightStateService.RemoveOverride(request.OverrideGuid);
        if (success)
        {
            return Ok();
        }
        return NotFound();
    }


    
    /// <summary>
    /// Get a combined state summary containing the next alarm event, bulb counts/status and current weather.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a small, cached snapshot intended for frequent polling by the UI.
    /// The payload is cheap to produce because the values are maintained by a background snapshot service.
    /// Clients may poll this endpoint every 1-2 seconds. The server may also return a 304 Not Modified
    /// if an ETag/Version mechanism is implemented in the snapshot service.
    /// </remarks>
    /// <returns>An <see cref="AlarmStateSummary"/> object containing combined state information.</returns>
    /// <response code="200">Returns the current state summary.</response>
    /// <response code="304">Not modified (if ETag/If-None-Match is supported and the snapshot hasn't changed).</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("state-summary")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(AlarmStateSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AlarmStateSummary>> GetAlarmStateSummary()
    {
        try
        {
            var summary = await _stateSummaryService.GetAlarmStateSummaryAsync();
            return Ok(summary);
        }
        catch (Exception e)
        {
            // Log the exception (not shown)
            return StatusCode(500, "Internal server error: " + e.Message + "\n" + e.StackTrace);
        }
    }
}
