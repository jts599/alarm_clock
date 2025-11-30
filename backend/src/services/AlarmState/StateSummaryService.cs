
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AlarmClock.Backend.DataModels.AlarmCore;

namespace AlarmClock.Backend.Services
{
    public class StateSummaryService : IStateSummaryService
    {

        private readonly ILightStateService _lightStateService;
        private readonly ILifxService _lifxService;

        public StateSummaryService(ILightStateService lightStateService, ILifxService lifxService)
        {
            _lightStateService = lightStateService;
            _lifxService = lifxService;
        }

        /// <summary>
        /// Get a summary of the current alarm state.
        /// </summary>
        /// <returns>A summary of the current alarm state.</returns>
        public async Task<AlarmStateSummary> GetAlarmStateSummaryAsync()
        {
            DateTime currentTime = await _lightStateService.GetScaledTime();
            var currentOverride = await _lightStateService.GetCurrentOverride();
            AlarmStateSummary summary = new AlarmStateSummary
            {
                // Example data; replace with actual logic to gather state summary
                CurrentTime = currentTime,
                NextAlarmEvent = (await _lightStateService.GetCurrentColorPickerCopy()).NextEvent(currentTime),
                LightOverrideState = currentOverride,
                NumberOfActiveBulbs = await _lifxService.GetNumberOfBulbsAsync()
            };
            return summary;
        }
    }
}