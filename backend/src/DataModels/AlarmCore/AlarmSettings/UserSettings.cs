namespace AlarmClock.Backend.DataModels.AlarmCore
{
    public class UserSettings
    {
        public int AlarmTimeInMinutesSinceMidnight { get; set; }
        public int TransitionMinutes { get; set; }
        public int TurnOffAfterMinutes { get; set; }
        public string[] EnabledDaysOfWeek { get; set; }
    }
}
