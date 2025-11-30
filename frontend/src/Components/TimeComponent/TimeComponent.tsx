import React, { useState, useEffect } from 'react'
import './TimeComponent.css'
import { useCurrentTime } from '../../hooks/useStateSummary'
import { isARealError } from '../../Clients/StateClient'

export const TimeComponent: React.FC = () => {
    const { data, isLoading, isError, error } = useCurrentTime(1000)

    const formatTime = (date: Date) => {
        const hours = date.getHours()
        const minutes = date.getMinutes()
        const isPM = hours >= 12
        const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours
        const displayMinutes = minutes.toString().padStart(2, '0')
        const dateDisplay = date.toLocaleDateString('en-US', { weekday: 'short', month: 'short', day: 'numeric' })
        
        return {
            time: `${displayHours}:${displayMinutes}`,
            period: isPM ? 'PM' : 'AM',
            dateDisplay
        }
    }

    let time: string
    let period: string
    let dateDisplay: string

    if (isLoading || isARealError(isError, error) || !data) {
        ({time, period, dateDisplay} = formatTime(new Date()))
    }
    else {
        ({time, period, dateDisplay} = formatTime(data))
    }

    return (
        <div className="time-component" style={{flexDirection: "column"}}>
            <div className="time-display">
                <span className="time-digits">{time}</span>
                <span className="time-period">{period}</span>
            </div>
            <div className="date-display">{dateDisplay}</div>
        </div>
    )
}

export default TimeComponent