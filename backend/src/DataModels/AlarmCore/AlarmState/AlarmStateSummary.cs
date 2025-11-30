using System;
using AlarmClock.Backend.DataModels.Weather;

namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class AlarmStateSummary
    {
        /// <summary>
        /// Number of bulbs that are currently active in the alarm system
        /// </summary>
        public int NumberOfActiveBulbs { get; set; }

        /// <summary>
        /// This is not real-time, this is the simulated current time based on the RunConfiguration.StartTimeIso8601 and TimescaleMultiplier
        /// </summary>
        public DateTime CurrentTime { get; set; }

        /// <summary>
        /// Information about the next scheduled alarm event
        /// </summary>
        public AlarmEventInfo NextAlarmEvent { get; set; }


        /// <summary>
        /// Info about currently applied override and whether the light is currently on
        /// </summary>
        public LightOverrideState LightOverrideState { get; set; }

    }
}