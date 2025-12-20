import React, { useState } from 'react'
import './DaysOfWeekPicker.css'

interface DaysOfWeekPickerProps {
  selectedDays?: string[]
  onChange?: (selectedDays: string[]) => void
}

export const DaysOfWeekPicker: React.FC<DaysOfWeekPickerProps> = ({ 
  selectedDays = [], 
  onChange 
}) => {
  const [internalSelectedDays, setInternalSelectedDays] = useState<string[]>(selectedDays)

  const days = [
    { initial: 'Su', full: 'Sunday' },
    { initial: 'M', full: 'Monday' },
    { initial: 'T', full: 'Tuesday' },
    { initial: 'W', full: 'Wednesday' },
    { initial: 'Th', full: 'Thursday' },
    { initial: 'F', full: 'Friday' },
    { initial: 'Sa', full: 'Saturday' }
  ]

  const handleDayToggle = (dayFull: string) => {
    const updatedDays = internalSelectedDays.includes(dayFull)
      ? internalSelectedDays.filter(day => day !== dayFull)
      : [...internalSelectedDays, dayFull]
    
    setInternalSelectedDays(updatedDays)
    onChange?.(updatedDays)
  }

  return (
    <div className="days-of-week-picker">
      <div className="days-container">
        {days.map((day, index) => (
          <button
            key={index}
            className={`day-button ${internalSelectedDays.includes(day.full) ? 'selected' : ''}`}
            onClick={() => handleDayToggle(day.full)}
            title={day.full}
          >
            {day.initial}
          </button>
        ))}
      </div>
    </div>
  )
}

export default DaysOfWeekPicker
