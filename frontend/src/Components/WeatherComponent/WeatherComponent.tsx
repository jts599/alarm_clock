import React, { useState, useEffect } from 'react'
import { GetWeatherClient, IWeatherClient, IForecast, WeatherCondition } from '../../Clients/WeatherClient'
import './WeatherComponent.css'

export const WeatherComponent: React.FC = () => {
    const [forecast, setForecast] = useState<IForecast | null>(null)
    const [isLoading, setIsLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    const weatherClient: IWeatherClient = GetWeatherClient()

    useEffect(() => {
        const fetchWeatherData = async () => {
            try {
                setIsLoading(true)
                setError(null)
                const forecastData = await weatherClient.getTwelveHourForecast()
                setForecast(forecastData)
            } catch (err) {
                setError('Failed to load weather data')
                console.error('Weather data fetch error:', err)
            } finally {
                setIsLoading(false)
            }
        }

        fetchWeatherData()

        // Refresh weather data every 30 minutes
        const refreshInterval = setInterval(fetchWeatherData, 30 * 60 * 1000)

        return () => clearInterval(refreshInterval)
    }, [])

    const getWeatherIcon = (condition: WeatherCondition): string => {
        switch (condition) {
            case WeatherCondition.Sunny: return '☀️'
            case WeatherCondition.Cloudy: return '☁️'
            case WeatherCondition.Rainy: return '🌧️'
            case WeatherCondition.Snowy: return '❄️'
            case WeatherCondition.Windy: return '💨'
            case WeatherCondition.Overcast: return '☁️'
            case WeatherCondition.Thunderstorm: return '⛈️'
            case WeatherCondition.Foggy: return '🌫️'
            default: return '🌤️'
        }
    }

    if (isLoading) {
        return (
            <div className="weather-component loading">
                <div className="weather-loading">Loading weather...</div>
            </div>
        )
    }

    if (error) {
        return (
            <div className="weather-component error">
                <div className="weather-error">{error}</div>
            </div>
        )
    }

    if (!forecast) {
        return (
            <div className="weather-component error">
                <div className="weather-error">No weather data available</div>
            </div>
        )
    }

    return (
        <div className="weather-component">
            
            <div className="weather-content">
                <div className="weather-icon">
                    {getWeatherIcon(forecast.weatherCondition)}
                </div>
                
                <div className="weather-details">
                    <div className="weather-condition">
                        {forecast.weatherCondition}
                    </div>
                    
                    <div className="temperature-range">
                        <span className="high-temp">{forecast.highTemp}°</span>
                        <span className="temp-separator">/</span>
                        <span className="low-temp">{forecast.lowTemp}°</span>
                    </div>
                    
                    <div className="precipitation">
                        <span className="precipitation-icon">🌧️</span>
                        <span className="precipitation-chance">{forecast.precipitationChance}%</span>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default WeatherComponent