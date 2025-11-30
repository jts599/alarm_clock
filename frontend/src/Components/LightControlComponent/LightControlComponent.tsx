import React, { useState } from 'react'
import { AlarmStateApi } from '../../../../shared/api/generated/apis/AlarmStateApi'
import { useViewController, Activities } from '../../contexts'
import './LightControlComponent.css'
import { CreateLightOverrideRequest } from '../../../../shared/api/generated'
import {  alarmApi } from '../../Clients/StateClient'
import { Icon, Icons } from '../Icon'
import { useActiveBulbsCount, useOverrideStatus } from '../../hooks/useStateSummary'

export const LightControlComponent: React.FC = () => {
    const { navigateTo } = useViewController()
    const [duration, setDuration] = useState<number>(5) // Default 5 minutes
    const [expectedOverrideId, setExpectedOverrideId] = useState<string | null | undefined>(undefined)

    const state = useOverrideStatus(1000)
    const currentOverrideId = state.data?.overrideGuid ?? null

    // Clear the loading state once the polled state matches our expectation
    React.useEffect(() => {
        if (expectedOverrideId !== undefined && currentOverrideId === expectedOverrideId) {
            setExpectedOverrideId(undefined)
        }
    }, [currentOverrideId, expectedOverrideId])

    const isLoading = expectedOverrideId !== undefined

    const handleTurnOnLight = async () => {
        try {
            const turnOnUntilRequest: CreateLightOverrideRequest = {
                minsToOverride: duration,
            }
            const response = await alarmApi.apiAlarmStateTurnOnUntilPost({ createLightOverrideRequest: turnOnUntilRequest })
            setExpectedOverrideId(response.overrideGuid ?? null)
        } catch (err) {
            console.error('Failed to turn on light:', err)
            setExpectedOverrideId(undefined) // Clear loading on error
        }
    }

    const handleTurnOffLight = async () => {
        try {
            if (currentOverrideId) {
                await alarmApi.apiAlarmStateRemoveOverrideDelete({removeAlarmOverrideRequest: {overrideGuid: currentOverrideId }})
                setExpectedOverrideId(null)
            }
        } catch (err) {
            console.error('Failed to turn off light override:', err)
            setExpectedOverrideId(undefined) // Clear loading on error
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

    const lightIconName = currentOverrideId ? Icons.Lights.LIGHTS_OFF : Icons.Lights.LIGHTS_ON
    const buttonAction = currentOverrideId ? handleTurnOffLight : handleTurnOnLight

    return (
        <div className="light-control-component">
            <div className="control-container">
                <button 
                    className="light-on-button"
                    onClick={buttonAction}
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