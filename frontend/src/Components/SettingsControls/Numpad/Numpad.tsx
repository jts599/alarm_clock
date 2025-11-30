import React, { useState } from 'react'
import './Numpad.css'

interface NumpadProps {
  value: number
  onConfirm: (value: number) => void
  onCancel: () => void
  min?: number
  max?: number
  label?: string
  description?: string
}

export const Numpad: React.FC<NumpadProps> = ({
  value,
  onConfirm,
  onCancel,
  min = 0,
  max = 999,
  label = 'Enter Value',
  description
}) => {
  const [displayValue, setDisplayValue] = useState<string>(value.toString())

  const handleNumberClick = (num: string) => {
    if (displayValue === '0') {
      setDisplayValue(num)
    } else if (displayValue.length < 3) { // Max 3 digits for reasonable minute values
      setDisplayValue(displayValue + num)
    }
  }

  const handleBackspace = () => {
    if (displayValue.length > 1) {
      setDisplayValue(displayValue.slice(0, -1))
    } else {
      setDisplayValue('0')
    }
  }

  const handleClear = () => {
    setDisplayValue('0')
  }

  const handleConfirm = () => {
    let numValue = parseInt(displayValue) || 0
    
    // Clamp to min/max
    if (numValue < min) numValue = min
    if (numValue > max) numValue = max
    
    onConfirm(numValue)
  }

  const handleOverlayClick = (e: React.MouseEvent) => {
    // Only close if clicking the overlay itself, not the numpad
    if (e.target === e.currentTarget) {
      onCancel()
    }
  }

  return (
    <div className="numpad-overlay" onClick={handleOverlayClick}>
      <div className="numpad-container">
        <div className="numpad-header">
          <span className="numpad-label">{label}</span>
          {description && <p className="numpad-description">{description}</p>}
          <div className="numpad-display">{displayValue}</div>
          <div className="numpad-range">({min}-{max} minutes)</div>
        </div>

        <div className="numpad-grid">
          <button className="numpad-button" onClick={() => handleNumberClick('7')}>7</button>
          <button className="numpad-button" onClick={() => handleNumberClick('8')}>8</button>
          <button className="numpad-button" onClick={() => handleNumberClick('9')}>9</button>
          
          <button className="numpad-button" onClick={() => handleNumberClick('4')}>4</button>
          <button className="numpad-button" onClick={() => handleNumberClick('5')}>5</button>
          <button className="numpad-button" onClick={() => handleNumberClick('6')}>6</button>
          
          <button className="numpad-button" onClick={() => handleNumberClick('1')}>1</button>
          <button className="numpad-button" onClick={() => handleNumberClick('2')}>2</button>
          <button className="numpad-button" onClick={() => handleNumberClick('3')}>3</button>
          
          <button className="numpad-button numpad-clear" onClick={handleClear}>C</button>
          <button className="numpad-button" onClick={() => handleNumberClick('0')}>0</button>
          <button className="numpad-button numpad-backspace" onClick={handleBackspace}>⌫</button>
        </div>

        <div className="numpad-actions">
          <button className="numpad-action-button numpad-cancel" onClick={onCancel}>
            Cancel
          </button>
          <button className="numpad-action-button numpad-confirm" onClick={handleConfirm}>
            Confirm
          </button>
        </div>
      </div>
    </div>
  )
}

export default Numpad
