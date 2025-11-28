import React, { useState, useEffect } from 'react'
import { useWeather } from '../../hooks/useStateSummary'
import './WeatherComponent.css'
import { isARealError } from '../../Clients/StateClient'
import { SingleDayForecast, WeatherResponse } from '../../../../shared/api/generated'
import { Icon } from '../Icon'

export const WeatherComponent: React.FC = () => {
    const { data: forecast, isLoading, isError, error } = useWeather()

    if (isLoading) {
        return (
            <div className="weather-component loading">
                <div className="weather-loading">Loading weather...</div>
            </div>
        )
    }


    if (shouldShowErrorCase(isError, error, forecast)) {
        console.error('Error fetching weather data:', error);
        return <></>;
    }

    //The bang operator is safe here because of the check above
    const todaysForecast: SingleDayForecast | undefined | null = forecast?.forecasts![0];

    if (!todaysForecast) {
        return <></>;
    }

    return (
        <div className="weather-component">
            <div className="weather-content">
                <div className="weather-icon">
                    <Icon name={todaysForecast.iconName!} size={48} />
                </div>

                <div className="weather-details">
                    <div className="temperature-range">
                        <span className="high-temp">{todaysForecast.highTemperatureF}°</span>
                        <span className="temp-separator">/</span>
                        <span className="low-temp">{todaysForecast.lowTemperatureF}°</span>
                    </div>

                    <div className="precipitation">
                        <span className="precipitation-icon">🌧️</span>
                        <span className="precipitation-chance">00%</span>
                    </div>
                </div>
            </div>
        </div>
    )
}

function shouldShowErrorCase(isError: boolean, error: Error | null, forecast: WeatherResponse | undefined): boolean {
    if (isARealError(isError, error) || forecast === undefined || forecast.forecasts?.length === 0) {
        return true;
    }
    return false;
}

export default WeatherComponent