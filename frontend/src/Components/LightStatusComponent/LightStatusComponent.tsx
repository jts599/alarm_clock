import React, { useState, useEffect } from 'react'
import { GetAlarmClient, IAlarmClient } from '../../Clients/AlarmClients'
import './LightStatusComponent.css'

export const LightStatusComponent: React.FC = () => {
    const [activeBulbs, setActiveBulbs] = useState<number | null>(null)
    const [isLoading, setIsLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    const alarmClient: IAlarmClient = GetAlarmClient()

    useEffect(() => {
        const fetchLightStatus = async () => {
            try {
                setIsLoading(true)
                setError(null)
                const bulbCount = await alarmClient.getNumberOfActiveBulbs()
                setActiveBulbs(bulbCount)
            } catch (err) {
                setError('Failed to load light status')
                console.error('Light status fetch error:', err)
            } finally {
                setIsLoading(false)
            }
        }

        fetchLightStatus()

        // Refresh light status every 30 seconds
        const refreshInterval = setInterval(fetchLightStatus, 30 * 1000)

        return () => clearInterval(refreshInterval)
    }, [])

    if (isLoading) {
        return (
            <div className="light-status-component loading">
                <div className="light-status-loading">Loading...</div>
            </div>
        )
    }

    if (error) {
        return (
            <div className="light-status-component error">
                <div className="light-status-error">{error}</div>
            </div>
        )
    }

    return (
        <div className="light-status-component">
            <div className="light-status-content">
                <div className="light-icon">
                    💡
                </div>
                <div className="light-count">
                    {activeBulbs}
                </div>
            </div>
        </div>
    )
}

export default LightStatusComponent