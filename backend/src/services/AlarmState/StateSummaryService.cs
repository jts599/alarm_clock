
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
            AlarmStateSummary summary = new AlarmStateSummary
            {
                // Example data; replace with actual logic to gather state summary
                CurrentTime = currentTime,
                NextAlarmEvent = (await _lightStateService.GetCurrentColorPickerCopy()).NextEvent(currentTime),
                NumberOfActiveBulbs = await _lifxService.GetNumberOfBulbsAsync(),
                WeatherForecast = null // Placeholder; integrate with weather service as needed
            };
            return summary;
        }
    }
}