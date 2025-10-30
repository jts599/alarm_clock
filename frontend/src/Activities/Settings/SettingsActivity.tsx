import React, { useState } from 'react'
import { Activities, IActivityProps } from '../../App'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'
import { StaticTimePicker } from '@mui/x-date-pickers/StaticTimePicker'
import { ThemeProvider, createTheme } from '@mui/material'
import dayjs, { Dayjs } from 'dayjs'
import DaysOfWeekPicker from '../../Components/SettingsControls/DaysOfWeekPicker'
import DurationInputs from '../../Components/SettingsControls/DurationInputs'
import './SettingsActivity.css'

export const SettingsActivity: React.FC<IActivityProps> = ({ activeActivitySetter }) => {
  const [alarmTime, setAlarmTime] = useState<Dayjs | null>(dayjs().hour(7).minute(0))
  const [selectedDays, setSelectedDays] = useState<string[]>(['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'])
  const [transitionLength, setTransitionLength] = useState<number>(30)
  const [daylightTime, setDaylightTime] = useState<number>(15)

  const handleBackClick = () => {
    activeActivitySetter(Activities.main)
  }

  const handleTimeChange = (newTime: Dayjs | null) => {
    setAlarmTime(newTime)
    if (newTime) {
      console.log('New alarm time:', newTime.format('HH:mm'))
      // TODO: Save to settings client
    }
  }

  const handleDaysChange = (days: string[]) => {
    setSelectedDays(days)
    console.log('Selected days:', days)
    // TODO: Save to settings client
  }

  const handleTransitionLengthChange = (value: number) => {
    setTransitionLength(value)
    console.log('Transition length:', value, 'minutes')
    // TODO: Save to settings client
  }

  const handleDaylightTimeChange = (value: number) => {
    setDaylightTime(value)
    console.log('Daylight time:', value, 'minutes')
    // TODO: Save to settings client
  }

  const handleSave = () => {
    const settings = {
      alarmTime: alarmTime?.format('HH:mm'),
      selectedDays,
      transitionLength,
      daylightTime
    }
    console.log('Saving settings:', settings)
    // TODO: Implement actual save to settings client
    // Show success message or navigate back
    activeActivitySetter(Activities.main)
  }

  // Create a dark theme for Material-UI components
  const darkTheme = createTheme({
    palette: {
      mode: 'dark',
      primary: {
        main: '#ffffff',
      },
      background: {
        default: 'transparent',
        paper: 'transparent',
      },
      text: {
        primary: '#ffffff',
        secondary: 'rgba(255, 255, 255, 0.7)',
      },
    },
    typography: {
      fontFamily: 'Comfortaa, Comic Sans MS, Segoe UI, Roboto, sans-serif',
      fontSize: 16,
    },
    components: {
      MuiTextField: {
        styleOverrides: {
          root: {
            '& .MuiOutlinedInput-root': {
              '& fieldset': {
                borderColor: 'rgba(255, 255, 255, 0.3)',
              },
              '&:hover fieldset': {
                borderColor: 'rgba(255, 255, 255, 0.5)',
              },
              '&.Mui-focused fieldset': {
                borderColor: '#ffffff',
              },
            },
          },
        },
      },
      MuiDialog: {
        styleOverrides: {
          paper: {
            backgroundColor: 'transparent',
            border: 'none',
          },
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            backgroundColor: 'transparent !important',
            backgroundImage: 'none !important',
          },
        },
      },
      MuiTypography: {
        styleOverrides: {
          root: {
            fontFamily: 'Comfortaa, Comic Sans MS, Segoe UI, Roboto, sans-serif',
          },
        },
      },
    },
  })

  return (
    <div className="settings-activity">
      <div className="settings-header">
      </div>
      
      <div className="settings-content">
        <div className="setting-section time-picker">
          <h2 className="centered-title">Alarm Time</h2>
          <ThemeProvider theme={darkTheme}>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <StaticTimePicker
                value={alarmTime}
                onChange={handleTimeChange}
                ampm={true}
                views={['hours', 'minutes']}
                openTo="hours"
                slotProps={{
                  actionBar: { actions: [] }
                }}
              />
            </LocalizationProvider>
          </ThemeProvider>
        </div>
        
        <div className="setting-section light-settings">
          <DurationInputs
            transitionLength={transitionLength}
            daylightTime={daylightTime}
            onTransitionLengthChange={handleTransitionLengthChange}
            onDaylightTimeChange={handleDaylightTimeChange}
          />
          
          <div className="days-section">
            <h3>Repeat Days</h3>
            <DaysOfWeekPicker
              selectedDays={selectedDays}
              onChange={handleDaysChange}
            />
          </div>
        </div>
      </div>
      
      <div className="action-buttons">
        <button className="cancel-button" onClick={handleBackClick}>
          <span className="button-icon">✕</span>
          <span className="button-text">Cancel</span>
        </button>
        <button className="save-button" onClick={handleSave}>
          <span className="button-icon">💾</span>
          <span className="button-text">Save</span>
        </button>
      </div>
    </div>
  )
}

export default SettingsActivity