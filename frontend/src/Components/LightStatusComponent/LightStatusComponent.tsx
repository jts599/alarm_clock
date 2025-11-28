import React, { useState, useEffect } from 'react'
import './LightStatusComponent.css'
import { useActiveBulbsCount, useNextAlarmEvent } from '../../hooks/useStateSummary'
import { isARealError } from '../../Clients/StateClient'

export const LightStatusComponent: React.FC = () => {

    const { data, error, isError, isLoading} = useActiveBulbsCount(1000);

    if (isARealError(isError, error)) {
        if (error) {
            console.error('Error fetching active bulbs count:', error);
        }
        return (
            <></>
        )
    }
    return (
        <LightStatusComponentContainer isLoading={isLoading} nBulbs={data ?? 0} />
    )
}

/**
 * Component container for light status
 * @param param0 isLoading and number of bulbs
 * @returns 
 */
const LightStatusComponentContainer: React.FC<{ isLoading: boolean; nBulbs: number }> = ({ isLoading, nBulbs }) => {
    return (
        <div className="light-status-component">
            {isLoading ? (
                <div className="light-status-content">
                    Loading... {/*TODO: Add spinner */}
                </div>
            ) : (
                <LightStatusComponentDisplay nBulbs={nBulbs} />
            )}
        </div>
    )
}

const LightStatusComponentDisplay: React.FC<{ nBulbs: number }> = ({ nBulbs }) => {
    return (
        <div className="light-status-content">
            <div className="light-icon">
                💡
            </div>
            <div className="light-count">
                {nBulbs}
            </div>
        </div>
    )
}

export default LightStatusComponent