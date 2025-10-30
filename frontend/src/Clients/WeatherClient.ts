export function GetWeatherClient(): IWeatherClient {
    return new StubWeatherClient()
}


export interface IWeatherClient {
    getTwelveHourForecast(): Promise<IForecast>
}

export interface IForecast {
    highTemp: number
    lowTemp: number
    precipitationChance: number
    weatherCondition: WeatherCondition
}

export enum WeatherCondition {
    Sunny = 'Sunny',
    Cloudy = 'Cloudy',
    Rainy = 'Rainy',
    Snowy = 'Snowy',
    Windy = 'Windy',
    Overcast = 'Overcast',
    Thunderstorm = 'Thunderstorm',
    Foggy = 'Foggy'
}

export class StubWeatherClient implements IWeatherClient {
    async getTwelveHourForecast(): Promise<IForecast> {
        return {
            highTemp: 75,
            lowTemp: 55,
            precipitationChance: 10,
            weatherCondition: WeatherCondition.Sunny
        }
    }
}

//todo implement a real weather client that fetches data from a weather API