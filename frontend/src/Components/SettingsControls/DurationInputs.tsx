import React, { useState } from 'react'
import Tooltip from '../UI/Tooltip'
import './DurationInputs.css'

interface DurationInputsProps {
  transitionLength?: number
  daylightTime?: number
  onTransitionLengthChange?: (value: number) => void
  onDaylightTimeChange?: (value: number) => void
}

export const DurationInputs: React.FC<DurationInputsProps> = ({
  transitionLength = 30,
  daylightTime = 15,
  onTransitionLengthChange,
  onDaylightTimeChange
}) => {
  const [internalTransitionLength, setInternalTransitionLength] = useState<number>(transitionLength)
  const [internalDaylightTime, setInternalDaylightTime] = useState<number>(daylightTime)

  const handleTransitionLengthChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value) || 0
    setInternalTransitionLength(value)
    onTransitionLengthChange?.(value)
  }

  const handleDaylightTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseInt(e.target.value) || 0
    setInternalDaylightTime(value)
    onDaylightTimeChange?.(value)
  }

  return (
    <div className="duration-inputs">
      <div className="input-group">
        <label htmlFor="transition-length" className="input-label">
          Transition Length
          <Tooltip 
            text="How long the lights take to gradually brighten from off to full brightness"
            position="bottom"
          >
            <span className="tooltip-icon">ℹ️</span>
          </Tooltip>
        </label>
        <div className="input-with-unit">
          <input
            id="transition-length"
            type="number"
            min="1"
            max="120"
            value={internalTransitionLength}
            onChange={handleTransitionLengthChange}
            className="duration-input"
          />
          <span className="unit-label">minutes</span>
        </div>
      </div>

      <div className="input-group">
        <label htmlFor="daylight-time" className="input-label">
          Daylight Time
          <Tooltip 
            text="How long the lights stay at full brightness before starting to dim"
            position="bottom"
          >
            <span className="tooltip-icon">ℹ️</span>
          </Tooltip>
        </label>
        <div className="input-with-unit">
          <input
            id="daylight-time"
            type="number"
            min="1"
            max="180"
            value={internalDaylightTime}
            onChange={handleDaylightTimeChange}
            className="duration-input"
          />
          <span className="unit-label">minutes</span>
        </div>
      </div>
    </div>
  )
}

export default DurationInputs