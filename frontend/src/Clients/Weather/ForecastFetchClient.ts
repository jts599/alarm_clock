import { fetchWeatherApi } from "openmeteo";
import { MapWeatherCodeToIcon } from './WeatherIconMap';

export interface IDailyForecast {
    date: Date
    temperatureHigh: number
    temperatureLow: number
    precipProbability: number
    icon: string
}

export interface IWeatherRequestParams {
    latitude: number
    longitude: number
}

/**
 * Fetches the daily weather forecast for the given longitude and latitude.
 * @param longLat 
 * @returns 
 */
export async function FetchDailyForecast(longLat: IWeatherRequestParams): Promise<IDailyForecast[]> {
    const response = await MakeWeatherRequest(longLat);

    const dailyForecasts: IDailyForecast[] = [];

    if (response.dates.length === 0){
        return dailyForecasts
    }
    for (let i = 0; i < response.dates.length; i++) {
        dailyForecasts.push({
            date: response.dates[i],
            temperatureHigh: response.temperatureMax ? response.temperatureMax[i] : NaN,
            temperatureLow: response.temperatureMin ? response.temperatureMin[i] : NaN,
            precipProbability: response.precipitationProbabilityMax ? response.precipitationProbabilityMax[i] : NaN,
            icon: MapWeatherCodeToIcon(response.weatherCode ? response.weatherCode[i] : -1, response.dates[i]),
        });
    }

    return dailyForecasts;

}





interface IOpenMeteoResponse {
    dates: Date[]
    temperatureMax?: number[]
    temperatureMin?: number[]
    precipitationProbabilityMax?: number[]
    weatherCode?: number[]
}

/**
 * Makes a weather request to the Open-Meteo API and returns the processed response.
 * @param longLat longitude and latitude for the weather request
 * @returns response from Open-Meteo API
 */
async function MakeWeatherRequest(longLat: IWeatherRequestParams): Promise<IOpenMeteoResponse> {
    const params = {
        latitude: longLat.latitude,
        longitude: longLat.longitude,
        daily: ["weather_code", "temperature_2m_max", "temperature_2m_min", "precipitation_probability_max"],
        hourly: ["temperature_2m", "precipitation_probability"],
        timezone: "auto",
        wind_speed_unit: "mph",
        temperature_unit: "fahrenheit",
        precipitation_unit: "inch",
    };
    const url = "https://api.open-meteo.com/v1/forecast";
    const responses = await fetchWeatherApi(url, params);

    // Process first location. Add a for-loop for multiple locations or weather models
    const response = responses[0];

    // Attributes for timezone and location
    const latitude = response.latitude();
    const longitude = response.longitude();
    const elevation = response.elevation();
    const timezone = response.timezone();
    const timezoneAbbreviation = response.timezoneAbbreviation();
    const utcOffsetSeconds = response.utcOffsetSeconds();

    console.log(
        `\nCoordinates: ${latitude}°N ${longitude}°E`,
        `\nElevation: ${elevation}m asl`,
        `\nTimezone: ${timezone} ${timezoneAbbreviation}`,
        `\nTimezone difference to GMT+0: ${utcOffsetSeconds}s`,
    );

    const hourly = response.hourly()!;
    const daily = response.daily()!;

    // Note: The order of weather variables in the URL query and the indices below need to match!
    const weatherData = {
        hourly: {
            time: Array.from(
                { length: (Number(hourly.timeEnd()) - Number(hourly.time())) / hourly.interval() }, 
                (_, i) => new Date((Number(hourly.time()) + i * hourly.interval() + utcOffsetSeconds) * 1000)
            ),
            temperature_2m: hourly.variables(0)!.valuesArray(),
            precipitation_probability: hourly.variables(1)!.valuesArray(),
        },
        daily: {
            time: Array.from(
                { length: (Number(daily.timeEnd()) - Number(daily.time())) / daily.interval() }, 
                (_, i) => new Date((Number(daily.time()) + i * daily.interval() + utcOffsetSeconds) * 1000)
            ),
            weather_code: daily.variables(0)!.valuesArray(),
            temperature_2m_max: daily.variables(1)!.valuesArray(),
            temperature_2m_min: daily.variables(2)!.valuesArray(),
            precipitation_probability_max: daily.variables(3)!.valuesArray(),
        },
    };

    const result: IOpenMeteoResponse = {
        dates: weatherData.daily.time,
        temperatureMax: weatherData.daily.temperature_2m_max ? Array.from(weatherData.daily.temperature_2m_max) : undefined,
        temperatureMin: weatherData.daily.temperature_2m_min ? Array.from(weatherData.daily.temperature_2m_min) : undefined,
        precipitationProbabilityMax: weatherData.daily.precipitation_probability_max ? Array.from(weatherData.daily.precipitation_probability_max) : undefined,
        weatherCode: weatherData.daily.weather_code ? Array.from(weatherData.daily.weather_code) : undefined,
    };

    return result;
}

