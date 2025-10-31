import React, { useState, useEffect } from 'react'
import { GetAlarmClient, IAlarmClient, IAlarmEventInfo, EventType } from '../../Clients/AlarmClients'
import './NextEventComponent.css'

export const NextEventComponent: React.FC = () => {
    const [eventInfo, setEventInfo] = useState<IAlarmEventInfo | null>(null)
    const [isLoading, setIsLoading] = useState<boolean>(true)
    const [error, setError] = useState<string | null>(null)

    const alarmClient: IAlarmClient = GetAlarmClient()

    useEffect(() => {
        const fetchEventData = async () => {
            try {
                setIsLoading(true)
                setError(null)
                const eventData = await alarmClient.getNextAlarmEventInfo()
                setEventInfo(eventData)
            } catch (err) {
                setError('Failed to load next event')
                console.error('Next event fetch error:', err)
            } finally {
                setIsLoading(false)
            }
        }

        fetchEventData()

        // Refresh event data every minute to keep it current
        const refreshInterval = setInterval(fetchEventData, 60 * 1000)

        return () => clearInterval(refreshInterval)
    }, [])

    const getEventIcon = (eventType: EventType): string => {
        switch (eventType) {
            case EventType.Sunrise:
                return '🌅'
            case EventType.LightOff:
                return '�' // Dim light symbol to indicate light turning off
            default:
                return '⏰'
        }
    }

    const getEventLabel = (eventType: EventType): string => {
        switch (eventType) {
            case EventType.Sunrise:
                return 'Sunrise'
            case EventType.LightOff:
                return 'Light Off'
            default:
                return 'Event'
        }
    }

    if (isLoading) {
        return (
            <div className="next-event-component loading">
                <div className="next-event-loading">Loading next event...</div>
            </div>
        )
    }

    if (error) {
        return (
            <div className="next-event-component error">
                <div className="next-event-error">{error}</div>
            </div>
        )
    }

    if (!eventInfo) {
        return (
            <div className="next-event-component error">
                <div className="next-event-error">No event data available</div>
            </div>
        )
    }

    return (
        <div className={`next-event-component ${eventInfo.isAlarmActive ? 'active' : 'inactive'}`}>
            <div className="next-event-content">
                <div className="event-icon">
                    {getEventIcon(eventInfo.nextEventType)}
                </div>
                <div>
                    
                    <div className="event-time">
                        {eventInfo.nextEventTime}
                    </div>
                    
                    <div className="event-day">
                        {eventInfo.nextEventDayOfWeek}
                    </div>
                </div>
            </div>
        </div>
    )
}

export default NextEventComponent