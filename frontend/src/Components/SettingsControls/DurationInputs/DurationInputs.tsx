import React, { useState } from 'react'
import { Numpad } from '../Numpad/Numpad'
import './DurationInputs.css'

interface DurationInputsProps {
  transitionLength?: number
  daylightTime?: number
  onTransitionLengthChange?: (value: number) => void
  onDaylightTimeChange?: (value: number) => void
}

type NumpadTarget = 'transition' | 'daylight' | null

export const DurationInputs: React.FC<DurationInputsProps> = ({
  transitionLength = 30,
  daylightTime = 15,
  onTransitionLengthChange,
  onDaylightTimeChange
}) => {
  const [internalTransitionLength, setInternalTransitionLength] = useState<number>(transitionLength)
  const [internalDaylightTime, setInternalDaylightTime] = useState<number>(daylightTime)
  const [activeNumpad, setActiveNumpad] = useState<NumpadTarget>(null)

  const handleTransitionLengthClick = () => {
    setActiveNumpad('transition')
  }

  const handleDaylightTimeClick = () => {
    setActiveNumpad('daylight')
  }

  const handleNumpadConfirm = (value: number) => {
    if (activeNumpad === 'transition') {
      setInternalTransitionLength(value)
      onTransitionLengthChange?.(value)
    } else if (activeNumpad === 'daylight') {
      setInternalDaylightTime(value)
      onDaylightTimeChange?.(value)
    }
    setActiveNumpad(null)
  }

  const handleNumpadCancel = () => {
    setActiveNumpad(null)
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

      {activeNumpad === 'transition' && (
        <Numpad
          value={internalTransitionLength}
          onConfirm={handleNumpadConfirm}
          onCancel={handleNumpadCancel}
          min={1}
          max={120}
          label="Transition Length"
          description="How long the lights take to gradually brighten from off to full brightness"
        />
      )}

      {activeNumpad === 'daylight' && (
        <Numpad
          value={internalDaylightTime}
          onConfirm={handleNumpadConfirm}
          onCancel={handleNumpadCancel}
          min={1}
          max={180}
          label="Daylight Time"
          description="How long the lights stay at full brightness before starting to dim"
        />
      )}
    </div>
  )
}

export default DurationInputs