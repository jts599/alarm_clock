namespace AlarmClock.Backend.Configuration
{
    public class WeatherConfiguration
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public class RunConfiguration
    {
        public bool StubLifx { get; set; }
        public bool StubBrightness { get; set; }
        public int TimescaleMultiplier { get; set; }

        public string StartTimeIso8601 { get; set; }
    }

    // Configuration that can be updated at runtime and persisted to a separate JSON file
    public class AlarmTimeConfiguration
    {
        // Minutes since midnight for the alarm start time (e.g. 6:30 AM -> 6*60 + 30 = 390)
        public int StartTimeInMinutesSinceMidnight { get; set; }

        // How long the transition (sunrise simulation) should take, in minutes
        public int TransitionDurationInMinutes { get; set; }

        // How long to stay on after the transition completes, in minutes
        public int StayOnTimeInMinutes { get; set; }

        //Which days the alarm is active on
        public string[] ActiveDays { get; set; }
    }
}