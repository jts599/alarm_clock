namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class AlarmEventInfo
    {
        public string NextEventDayOfWeek { get; set; } = string.Empty;
        public string NextEventTime { get; set; } = string.Empty;
        public EventType NextEventType { get; set; }
        public bool IsAlarmActive { get; set; }
    }
}
