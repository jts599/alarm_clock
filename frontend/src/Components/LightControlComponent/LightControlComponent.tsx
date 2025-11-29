import React, { useState } from 'react'
import { AlarmStateApi } from '../../../../shared/api/generated/apis/AlarmStateApi'
import { useViewController, Activities } from '../../contexts'
import './LightControlComponent.css'
import { CreateLightOverrideRequest } from '../../../../shared/api/generated'
import {  alarmApi } from '../../Clients/StateClient'
import { Icon, Icons } from '../Icon'

export const LightControlComponent: React.FC = () => {
    const { navigateTo } = useViewController()
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
        navigateTo(Activities.settings)
    }

    const handleDurationAdjust = () => {
        // Cycle through common durations: 5, 10, 15, 30, 60 minutes
        const durations = [5, 10, 15, 30, 60]
        const currentIndex = durations.indexOf(duration)
        const nextIndex = (currentIndex + 1) % durations.length
        setDuration(durations[nextIndex])
    }

    const lightIconName = Icons.Lights.LIGHTS_ON

    return (
        <div className="light-control-component">
            <div className="control-container">
                <button 
                    className="light-on-button"
                    onClick={handleTurnOnLight}
                    disabled={isLoading}
                >
                    <div className="button-icon">
                        <Icon name={lightIconName} size={64} />
                    </div>
                </button>

                <div className="right-buttons">
                    <button 
                        className="settings-button"
                        onClick={handleSettings}
                    >
                        <div className="button-icon">
                            <Icon name={Icons.SETTINGS} size={48} />
                        </div>
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