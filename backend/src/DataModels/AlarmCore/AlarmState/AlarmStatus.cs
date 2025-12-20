using System;

namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class AlarmStatus
    {
        public DateTime CurrentTime { get; set; }
        public bool IsLightCurrentlyOn { get; set; }
        public Color CurrentColor { get; set; }
        public DateTime NextAlarmTime { get; set; }
        public TimeOnly AlarmTime { get; set; }
        public int TransitionMinutes { get; set; }
        public int HoldOnMinutes { get; set; }
        public string[] ActiveDays { get; set; }
    }
}
