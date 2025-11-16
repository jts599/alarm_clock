using System;

namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class TurnLightOnRequest
    {
        public DateTime NextEventTime { get; set; }
    }
}
