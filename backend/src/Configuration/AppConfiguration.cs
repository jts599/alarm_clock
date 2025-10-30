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
        public int TimescaleMultiplier { get; set; }

        public int StartTimeHour { get; set; }
    }
}