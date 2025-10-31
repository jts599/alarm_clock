import React, { useState, useEffect } from 'react'
import './TimeComponent.css'

export const TimeComponent: React.FC = () => {
    const [currentTime, setCurrentTime] = useState<Date>(new Date())

    useEffect(() => {
        const updateTime = () => {
            setCurrentTime(new Date())
        }

        // Update immediately
        updateTime()

        // Update every second
        const timeInterval = setInterval(updateTime, 1000)

        return () => clearInterval(timeInterval)
    }, [])

    const formatTime = (date: Date) => {
        const hours = date.getHours()
        const minutes = date.getMinutes()
        const isPM = hours >= 12
        const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours
        const displayMinutes = minutes.toString().padStart(2, '0')
        
        return {
            time: `${displayHours}:${displayMinutes}`,
            period: isPM ? 'PM' : 'AM'
        }
    }

    const { time, period } = formatTime(currentTime)

    return (
        <div className="time-component">
            <div className="time-display">
                <span className="time-digits">{time}</span>
                <span className="time-period">{period}</span>
            </div>
        </div>
    )
}

export default TimeComponent