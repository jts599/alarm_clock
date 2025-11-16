import React, { useState, useEffect } from 'react'
import './TimeComponent.css'
import { useCurrentTime } from '../../hooks/useStateSummary'
import { isARealError } from '../../clients/StateClient'

export const TimeComponent: React.FC = () => {
    const { data, isLoading, isError, error } = useCurrentTime(1000)

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

    let time: string
    let period: string

    if (isLoading || isARealError(isError, error) || !data) {
        ({time, period} = formatTime(new Date()))
    }
    else {
        ({time, period} = formatTime(data))
    }

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