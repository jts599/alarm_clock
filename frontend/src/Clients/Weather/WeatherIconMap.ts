 /*
0	Clear sky
1, 2, 3	Mainly clear, partly cloudy, and overcast
45, 48	Fog and depositing rime fog
51, 53, 55	Drizzle: Light, moderate, and dense intensity
56, 57	Freezing Drizzle: Light and dense intensity
61, 63, 65	Rain: Slight, moderate and heavy intensity
66, 67	Freezing Rain: Light and heavy intensity
71, 73, 75	Snow fall: Slight, moderate, and heavy intensity
77	Snow grains
80, 81, 82	Rain showers: Slight, moderate, and violent
85, 86	Snow showers slight and heavy
95 *	Thunderstorm: Slight or moderate
96, 99 *	Thunderstorm with slight and heavy hail
    */
import { Icons } from '../../Components/Icon'


export function MapWeatherCodeToIcon(weatherCode: number, time: Date, sunriseTime: Date, sunsetTime: Date): string {
    // Mapping based on Open-Meteo weather codes
    const useNightIcon = (date: Date, sunriseTime: Date, sunsetTime: Date): boolean => {
        return date < sunriseTime || date >= sunsetTime;
    }

   
    if (useNightIcon(time, sunriseTime, sunsetTime)) {
        return nightTimeWeatherCodeMap[weatherCode] || Icons.Weather.NA
    } else {
        return DayTimeWeatherCodeMap[weatherCode] || Icons.Weather.NA
    } 
}


 /*
0	Clear sky
1, 2, 3	Mainly clear, partly cloudy, and overcast
45, 48	Fog and depositing rime fog
51, 53, 55	Drizzle: Light, moderate, and dense intensity
56, 57	Freezing Drizzle: Light and dense intensity
61, 63, 65	Rain: Slight, moderate and heavy intensity
66, 67	Freezing Rain: Light and heavy intensity
71, 73, 75	Snow fall: Slight, moderate, and heavy intensity
77	Snow grains
80, 81, 82	Rain showers: Slight, moderate, and violent
85, 86	Snow showers slight and heavy
95 *	Thunderstorm: Slight or moderate
96, 99 *	Thunderstorm with slight and heavy hail
    */
const DayTimeWeatherCodeMap: { [key: number]: string } = {
    // Clear sky
    0: Icons.Weather.Day.SUNNY,
    // Mainly clear, partly cloudy, and overcast
    1: Icons.Weather.Day.SUNNY_OVERCAST,
    2: Icons.Weather.Day.CLOUDY,
    3: Icons.Weather.CLOUDY,
    //Fog and Ice fog
    45: Icons.Weather.FOG,
    48: Icons.Weather.FOG,
    //Drizzle: Light, moderate and dense intensity
    51: Icons.Weather.Day.SPRINKLE,
    53: Icons.Weather.Day.RAIN,
    55: Icons.Weather.SHOWERS,
    //Freezing Drizzle: Light and dense intensity
    56: Icons.Weather.Day.SLEET,
    57: Icons.Weather.SLEET,
    //Rain slight,moderate and heavy intensity
    61: Icons.Weather.Day.SHOWERS,
    63: Icons.Weather.Day.RAIN,
    65: Icons.Weather.RAIN,
    //Freezing Rain: Light and heavy intensity
    66: Icons.Weather.Day.RAIN_MIX,
    67: Icons.Weather.RAIN_MIX,
    //Snow fall: Slight, moderate, and heavy intensity
    71: Icons.Weather.Day.SNOW,
    73: Icons.Weather.SNOW,
    75: Icons.Weather.SNOW_WIND,
    //Snow grains
    77: Icons.Weather.SNOWFLAKE_COLD,
    //Rain showers: Slight, moderate, and violent
    80: Icons.Weather.Day.SHOWERS,
    81: Icons.Weather.SHOWERS,
    82: Icons.Weather.Day.STORM_SHOWERS,
    //Snow showers slight and heavy
    85: Icons.Weather.Day.SNOW,
    86: Icons.Weather.SNOW,
    //Thunderstorm: Slight or moderate
    95: Icons.Weather.Day.THUNDERSTORM,
    96: Icons.Weather.Day.THUNDERSTORM,
    99: Icons.Weather.Day.THUNDERSTORM,
}



 /*
0	Clear sky
1, 2, 3	Mainly clear, partly cloudy, and overcast
45, 48	Fog and depositing rime fog
51, 53, 55	Drizzle: Light, moderate, and dense intensity
56, 57	Freezing Drizzle: Light and dense intensity
61, 63, 65	Rain: Slight, moderate and heavy intensity
66, 67	Freezing Rain: Light and heavy intensity
71, 73, 75	Snow fall: Slight, moderate, and heavy intensity
77	Snow grains
80, 81, 82	Rain showers: Slight, moderate, and violent
85, 86	Snow showers slight and heavy
95 *	Thunderstorm: Slight or moderate
96, 99 *	Thunderstorm with slight and heavy hail
    */
const nightTimeWeatherCodeMap: { [key: number]: string } = {
    // Clear sky
    0: Icons.Weather.Night.STARS,
    // Mainly clear, partly cloudy, and overcast
    1: Icons.Weather.Night.PARTLY_CLOUDY,
    2: Icons.Weather.Night.CLOUDY,
    3: Icons.Weather.CLOUDY,
    //Fog and Ice fog
    45: Icons.Weather.FOG,
    48: Icons.Weather.FOG,
    //Drizzle: Light, moderate and dense intensity
    51: Icons.Weather.Night.SPRINKLE,
    53: Icons.Weather.Night.RAIN,
    55: Icons.Weather.SHOWERS,
    //Freezing Drizzle: Light and dense intensity
    56: Icons.Weather.Night.SLEET,
    57: Icons.Weather.SLEET,
    //Rain slight,moderate and heavy intensity
    61: Icons.Weather.Night.SHOWERS,
    63: Icons.Weather.Night.RAIN,
    65: Icons.Weather.RAIN,
    //Freezing Rain: Light and heavy intensity
    66: Icons.Weather.Night.RAIN_MIX,
    67: Icons.Weather.RAIN_MIX,
    //Snow fall: Slight, moderate, and heavy intensity
    71: Icons.Weather.Night.SNOW,
    73: Icons.Weather.SNOW,
    75: Icons.Weather.SNOW_WIND,
    //Snow grains
    77: Icons.Weather.SNOWFLAKE_COLD,
    //Rain showers: Slight, moderate, and violent
    80: Icons.Weather.Night.SHOWERS,
    81: Icons.Weather.SHOWERS,
    82: Icons.Weather.Night.STORM_SHOWERS,
    //Snow showers slight and heavy
    85: Icons.Weather.Night.SNOW,
    86: Icons.Weather.SNOW,
    //Thunderstorm: Slight or moderate
    95: Icons.Weather.Night.THUNDERSTORM,
    96: Icons.Weather.Night.THUNDERSTORM,
    99: Icons.Weather.Night.THUNDERSTORM,
}