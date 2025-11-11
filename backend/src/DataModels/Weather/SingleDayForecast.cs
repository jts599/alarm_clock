using System;

namespace AlarmClock.Backend.DataModels.Weather
{

    public enum DayOrNight
    {
        Day,
        Night
    }

    public class SingleDayForecast
    {
        public DateOnly ForecastDate { get; set; }
        public DayOrNight DayOrNight { get; set; }
        public int HighTemperatureF { get; set; }
        public int LowTemperatureF { get; set; }
        public string ShortForecast { get; set; }
        public string IconName { get; set; }
    }
}