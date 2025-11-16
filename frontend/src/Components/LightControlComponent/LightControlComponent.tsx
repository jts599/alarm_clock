import React, { useState } from 'react'
import { AlarmStateApi } from '../../../../shared/api/generated/apis/AlarmStateApi'
import { Activities, IActivityProps } from '../../App'
import './LightControlComponent.css'
import { CreateLightOverrideRequest } from '../../../../shared/api/generated'
import {  alarmApi } from '../../clients/StateClient'

export const LightControlComponent: React.FC<IActivityProps> = ({ activeActivitySetter }) => {
    const [isLoading, setIsLoading] = useState<boolean>(false)
    const [duration, setDuration] = useState<number>(5) // Default 5 minutes

    const handleTurnOnLight = async () => {
        try {
            setIsLoading(true)
            
            const turnOnUntilRequest: CreateLightOverrideRequest = {
                minsToOverride: duration,
            }
            await alarmApi.apiAlarmStateTurnOnUntilPost({ createLightOverrideRequest: turnOnUntilRequest })
        } catch (err) {
            console.error('Failed to turn on light:', err)
        } finally {
            setIsLoading(false)
        }
    }

    const handleSettings = () => {
        activeActivitySetter(Activities.settings)
    }

    const handleDurationAdjust = () => {
        // Cycle through common durations: 5, 10, 15, 30, 60 minutes
        const durations = [5, 10, 15, 30, 60]
        const currentIndex = durations.indexOf(duration)
        const nextIndex = (currentIndex + 1) % durations.length
        setDuration(durations[nextIndex])
    }

    return (
        <div className="light-control-component">
            <div className="control-container">
                <button 
                    className="light-on-button"
                    onClick={handleTurnOnLight}
                    disabled={isLoading}
                >
                    <div className="button-icon">💡</div>
                </button>

                <div className="right-buttons">
                    <button 
                        className="settings-button"
                        onClick={handleSettings}
                    >
                        <div className="button-icon">⚙️</div>
                    </button>

                    <button 
                        className="duration-button"
                        onClick={handleDurationAdjust}
                    >
                        <div className="button-text">{duration}m</div>
                    </button>
                </div>
            </div>
        </div>
    )
}

export default LightControlComponent