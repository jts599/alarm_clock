import { fetchWeatherApi } from "openmeteo";
import { MapWeatherCodeToIcon } from './WeatherIconMap';

export interface IDailyForecast {
    date: Date
    temperatureHigh: number
    temperatureLow: number
    precipProbability: number
    icon: string
    sunriseTime: Date
    sunsetTime: Date
}

export interface IWeatherRequestParams {
    latitude: number
    longitude: number
}

export interface IWeatherResponse {
    dailyForecasts: IDailyForecast[]
    hourlyForecasts: IHourlyForecast[]
}

export interface IHourlyForecast {
    time: Date
    temperature: number
    precipProbability: number
    icon: string
    //These get used to determine whether to show day or night icons
    sunriseTime: Date
    sunsetTime: Date
}

/**
 * Fetches the daily weather forecast for the given longitude and latitude.
 * @param longLat 
 * @returns 
 */
export async function FetchForecast(longLat: IWeatherRequestParams): Promise<IWeatherResponse> {
    const response = await MakeWeatherRequest(longLat);

    const dailyForecasts: IDailyForecast[] = [];
    const hourlyForecasts: IHourlyForecast[] = [];

    if (response.daily.dates.length === 0){
        return { dailyForecasts, hourlyForecasts };
    }
    fillDailyForecasts(response.daily, dailyForecasts);
    fillHourlyForecasts(response.hourly, hourlyForecasts, dailyForecasts);

    return { dailyForecasts, hourlyForecasts };

}

function sanitizeTime(date: Date): Date {
    //if date is today, use current time
    const now = new Date();
    if (date.getDate() === now.getDate() &&
        date.getMonth() === now.getMonth() &&
        date.getFullYear() === now.getFullYear()) {
        return now;
    }
    //else use noon time
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 12, 0, 0);
}

/**
 * Transforms the Open-Meteo daily response into the internal daily forecast format.
 * @param response 
 * @param dailyForecasts 
 */
function fillDailyForecasts(response: IOpenMeteoDailyResponse, dailyForecasts: IDailyForecast[]) {
    for (let i = 0; i < response.dates.length; i++) {
        const sunriseTime = response.sunrise ? response.sunrise[i] : new Date();
        const sunsetTime = response.sunset ? response.sunset[i] : new Date();
        dailyForecasts.push({
            date: response.dates[i],
            temperatureHigh: response.temperatureMax ? response.temperatureMax[i] : NaN,
            temperatureLow: response.temperatureMin ? response.temperatureMin[i] : NaN,
            precipProbability: response.precipitationProbabilityMax ? response.precipitationProbabilityMax[i] : NaN,
            icon: MapWeatherCodeToIcon(response.weatherCode ? response.weatherCode[i] : -1, sanitizeTime(response.dates[i]), sunriseTime, sunsetTime),
            sunriseTime: sunriseTime,
            sunsetTime: sunsetTime,
        });
    }
}
/**
 * Gets the sunrise and sunset times for a specific date from the daily forecast data.
 * @param time 
 * @param dailyResponse 
 * @returns 
 */
function getSunriseSunsetForTime(time: Date, dailyResponse: IDailyForecast[]): {sunriseTime: Date, sunsetTime: Date} {
    const indexDate = time.getDate();
    for (let i = 0; i < dailyResponse.length; i++) {
        const dailyDate = dailyResponse[i].date;
        if (dailyDate.getDate() === indexDate) {
            return {
                sunriseTime: dailyResponse[i].sunriseTime,
                sunsetTime: dailyResponse[i].sunsetTime
            };
        }
    }
    return {
        sunriseTime: new Date(),
        sunsetTime: new Date()
    };
}

/**
 * Transforms the Open-Meteo hourly response into the internal hourly forecast format.
 * @param response 
 * @param hourlyForecasts 
 */
function fillHourlyForecasts(response: IOpenMeteoHourlyResponse, hourlyForecasts: IHourlyForecast[], dailyForecasts: IDailyForecast[]) {
    for (let i = 0; i < response.times.length; i++) {
        const { sunriseTime, sunsetTime } = getSunriseSunsetForTime(response.times[i], dailyForecasts); 
        hourlyForecasts.push({
            time: response.times[i],
            temperature: response.temperatures ? response.temperatures[i] : NaN,
            precipProbability: response.precipitationProbabilities ? response.precipitationProbabilities[i] : NaN,
            sunriseTime: sunriseTime,
            sunsetTime: sunsetTime,
            icon: MapWeatherCodeToIcon(response.weatherCodes ? response.weatherCodes[i] : -1, response.times[i], sunriseTime, sunsetTime),
        });
    }
}

interface IOpenMeteoResponse {
    daily: IOpenMeteoDailyResponse
    hourly: IOpenMeteoHourlyResponse
}

interface IOpenMeteoHourlyResponse {
    times: Date[]
    temperatures?: number[]
    precipitationProbabilities?: number[]
    weatherCodes?: number[]
}

interface IOpenMeteoDailyResponse {
    dates: Date[]
    temperatureMax?: number[]
    temperatureMin?: number[]
    precipitationProbabilityMax?: number[]
    weatherCode?: number[]
    sunrise?: Date[]
    sunset?: Date[]
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
        daily: ["weather_code", "temperature_2m_max", "temperature_2m_min", "precipitation_probability_max", "sunset", "sunrise"],
        hourly: ["temperature_2m", "precipitation_probability", "weather_code"],
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

    // Define Int64 variables so they can be processed accordingly
    const sunset = daily.variables(4)!;
    const sunrise = daily.variables(5)!;

    // Note: The order of weather variables in the URL query and the indices below need to match!
    const weatherData = {
        daily: {
            time: Array.from(
                { length: (Number(daily.timeEnd()) - Number(daily.time())) / daily.interval() }, 
                (_, i) => new Date((Number(daily.time()) + i * daily.interval()) * 1000)
            ),
            weather_code: daily.variables(0)!.valuesArray(),
            temperature_2m_max: daily.variables(1)!.valuesArray(),
            temperature_2m_min: daily.variables(2)!.valuesArray(),
            precipitation_probability_max: daily.variables(3)!.valuesArray(),
            sunsetTime: [...Array(sunset.valuesInt64Length())].map(
			    (_, i) => new Date((Number(sunset.valuesInt64(i))) * 1000)
            ),
            // Map Int64 values to according structure
            sunriseTime: [...Array(sunrise.valuesInt64Length())].map(
                (_, i) => new Date((Number(sunrise.valuesInt64(i))) * 1000)
            ),
        },
        hourly: {
            time: Array.from(
                { length: (Number(hourly.timeEnd()) - Number(hourly.time())) / hourly.interval() }, 
                (_, i) => new Date((Number(hourly.time()) + i * hourly.interval()) * 1000)
            ),
            temperature_2m: hourly.variables(0)!.valuesArray(),
            precipitation_probability: hourly.variables(1)!.valuesArray(),
            weather_code: hourly.variables(2)!.valuesArray(),
        },
    };

    const dailyResult: IOpenMeteoDailyResponse = {
        dates: weatherData.daily.time,
        temperatureMax: weatherData.daily.temperature_2m_max ? Array.from(weatherData.daily.temperature_2m_max) : undefined,
        temperatureMin: weatherData.daily.temperature_2m_min ? Array.from(weatherData.daily.temperature_2m_min) : undefined,
        precipitationProbabilityMax: weatherData.daily.precipitation_probability_max ? Array.from(weatherData.daily.precipitation_probability_max) : undefined,
        weatherCode: weatherData.daily.weather_code ? Array.from(weatherData.daily.weather_code) : undefined,
        sunrise: weatherData.daily.sunriseTime ? Array.from(weatherData.daily.sunriseTime) : undefined,
        sunset: weatherData.daily.sunsetTime ? Array.from(weatherData.daily.sunsetTime) : undefined,
    };

    const hourlyResult: IOpenMeteoHourlyResponse = {
        times: weatherData.hourly.time,
        temperatures: weatherData.hourly.temperature_2m ? Array.from(weatherData.hourly.temperature_2m) : undefined,
        precipitationProbabilities: weatherData.hourly.precipitation_probability ? Array.from(weatherData.hourly.precipitation_probability) : undefined,
        weatherCodes: weatherData.hourly.weather_code ? Array.from(weatherData.hourly.weather_code) : undefined,
    };


    return {        
        daily: dailyResult,
        hourly: hourlyResult,
    };
}

