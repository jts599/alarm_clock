import { WeatherApi } from '../../../../shared/api/generated/apis'
import { Configuration } from '../../../../shared/api/generated'
import { FetchForecast, IWeatherRequestParams, IWeatherResponse } from './ForecastFetchClient'

const config = new Configuration({
    basePath: ''
})

const weatherApi = new WeatherApi(config)

export async function fetchWeatherLocation(): Promise<IWeatherRequestParams> {
    const response = await weatherApi.apiWeatherLocationPost()
    return {
        latitude: response.latitude!,
        longitude: response.longitude!
    }
}

export async function fetchWeatherForecast(): Promise<IWeatherResponse> {
    const location = await fetchWeatherLocation()
    return await FetchForecast(location)
}
