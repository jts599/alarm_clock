import React from 'react'
import './WeatherComponent.css'
import { isARealError } from '../../Clients/StateClient'
import { useWeatherForecast } from '../../hooks/useWeatherForecast'
import { SingleDayDisplay, DailyDisplayMode } from './DailyDisplay'

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

    if (!todaysForecast) {
        return <></>
    }

    return (
        <div className="weather-component">
            <SingleDayDisplay singleDayForecast={todaysForecast} displayMode={DailyDisplayMode.StandaloneForecast} />
        </div>
    )
}

export default WeatherComponent