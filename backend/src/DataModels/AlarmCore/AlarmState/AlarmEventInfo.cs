using System;

namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class AlarmEventInfo
    {
        public string NextEventDayOfWeek { get; set; } = string.Empty;
        public string NextEventTime { get; set; } = string.Empty;
        public EventType NextEventType { get; set; }

        public AlarmEventInfo()
        {

        }

        public AlarmEventInfo(DateTime eventTime, EventType eventType)
        {
            NextEventDayOfWeek = eventTime.DayOfWeek.ToString();
            NextEventTime = eventTime.ToString("hh:mm tt");
            NextEventType = eventType;
        }
    }
}
