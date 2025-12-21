import React from 'react'
import './WeatherComponent.css'
import { isARealError } from '../../Clients/StateClient'
import { useWeatherForecast } from '../../hooks/useWeatherForecast'
import { CurrentWeatherDisplay } from './DailyDisplay'
import { getCurrentForecast } from '../../Clients/Weather/ForecastFetchClient'

export const WeatherComponent: React.FC = () => {
    const { data: forecast, isLoading, isError, error } = useWeatherForecast()

    if (isLoading) {
        return (
            <div className="weather-component loading">
                <div className="weather-loading">Loading weather...</div>
            </div>
        )
    }

    if (isARealError(isError, error) || !forecast) {
        console.error('Error fetching weather data:', error)
        return <></>
    }

    const todaysForecast = forecast.dailyForecasts[0]
    const nowWeather = getCurrentForecast(forecast.hourlyForecasts)

    if (!todaysForecast || !nowWeather) {
        return <></>
    }

    return (
        <div className="weather-component">
            <CurrentWeatherDisplay singleDayForecast={todaysForecast} currentHourlyForecast={nowWeather} />
        </div>
    )
}

export default WeatherComponent