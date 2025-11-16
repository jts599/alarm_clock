using System.Collections.Generic;

namespace AlarmClock.Backend.DataModels.Weather
{
    public class WeatherResponse
    {
        public List<SingleDayForecast> Forecasts { get; set; }
    }
}