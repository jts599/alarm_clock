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

    public class LlmConfiguration
    {
        public string ModelPath { get; set; } = "";
        public int MaxTokens { get; set; } = 100;
        public float Temperature { get; set; } = 0.1f;
        public bool EnableLlm { get; set; } = true;
    }
}