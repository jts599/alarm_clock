import React, { useState } from 'react'
import './DurationInputs.css'

interface DurationInputsProps {
  transitionLength?: number
  daylightTime?: number
  onTransitionLengthChange?: (value: number) => void
  onDaylightTimeChange?: (value: number) => void
  setActiveNumpad: (target: NumpadTarget) => void
  activeNumpad?: NumpadTarget
}

export type NumpadTarget = 'transition' | 'daylight' | null

export const DurationInputs: React.FC<DurationInputsProps> = ({
  transitionLength = 30,
  daylightTime = 15,
  onTransitionLengthChange,
  onDaylightTimeChange,
  setActiveNumpad,
  activeNumpad
}) => {
  const [internalTransitionLength, setInternalTransitionLength] = useState<number>(transitionLength)
  const [internalDaylightTime, setInternalDaylightTime] = useState<number>(daylightTime)


  const handleTransitionLengthClick = () => {
    setActiveNumpad('transition')
  }

  const handleDaylightTimeClick = () => {
    setActiveNumpad('daylight')
  }

  return (
    <div className="duration-inputs">
      <div className="input-group">
        <label htmlFor="transition-length" className="input-label">
          Transition Length
        </label>
        <div className="input-with-unit">
          <input
            id="transition-length"
            type="text"
            readOnly
            value={internalTransitionLength}
            onClick={handleTransitionLengthClick}
            className="duration-input duration-input-readonly"
          />
          <span className="unit-label">minutes</span>
        </div>
      </div>

      <div className="input-group">
        <label htmlFor="daylight-time" className="input-label">
          Daylight Time
        </label>
        <div className="input-with-unit">
          <input
            id="daylight-time"
            type="text"
            readOnly
            value={internalDaylightTime}
            onClick={handleDaylightTimeClick}
            className="duration-input duration-input-readonly"
          />
          <span className="unit-label">minutes</span>
        </div>
      </div>
    </div>
  )
}

export default DurationInputs