import { useQuery, UseQueryResult } from '@tanstack/react-query'
import { fetchWeatherForecast } from '../Clients/Weather/WeatherClient'
import { IWeatherResponse } from '../Clients/Weather/ForecastFetchClient'

const THIRTY_MINUTES_MS = 30 * 60 * 1000

export function useWeatherForecast(): UseQueryResult<IWeatherResponse, Error> {
    return useQuery<IWeatherResponse, Error>({
        queryKey: ['weatherForecast'],
        queryFn: fetchWeatherForecast,
        refetchInterval: THIRTY_MINUTES_MS,
        staleTime: THIRTY_MINUTES_MS,
        refetchOnWindowFocus: false,
        retry: 3,
    })
}
