import React, { useState, useEffect } from 'react'
import { useViewController, Activities } from '../../contexts'
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider'
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'
import { StaticTimePicker } from '@mui/x-date-pickers/StaticTimePicker'
import { ThemeProvider, createTheme } from '@mui/material'
import dayjs, { Dayjs } from 'dayjs'
import DaysOfWeekPicker from '../../Components/SettingsControls/DaysOfWeekPicker'
import DurationInputs from '../../Components/SettingsControls/DurationInputs'
import { GetSettingsClient, IUserSettings } from '../../Clients/SettingsClient'
import './SettingsActivity.css'

export const SettingsActivity: React.FC = () => {
  const { navigateTo } = useViewController()
  const [alarmTime, setAlarmTime] = useState<Dayjs | null>(dayjs().hour(7).minute(0))
  const [selectedDays, setSelectedDays] = useState<string[]>(['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday'])
  const [transitionLength, setTransitionLength] = useState<number>(30)
  const [daylightTime, setDaylightTime] = useState<number>(15)
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [isSaving, setIsSaving] = useState<boolean>(false)
  
  const settingsClient = GetSettingsClient()

  // Load settings from the server when component mounts
  useEffect(() => {
    const loadSettings = async () => {
      try {
        setIsLoading(true)
        const settings = await settingsClient.getUserSettings()
        
        // Convert minutes since midnight to dayjs time
        const totalMinutes = settings.AlarmTimeInMinutesSinceMidnight
        const hours = Math.floor(totalMinutes / 60)
        const minutes = totalMinutes % 60
        setAlarmTime(dayjs().hour(hours).minute(minutes))
        
        setSelectedDays(settings.enabledDaysOfWeek)
        setTransitionLength(settings.transitionMinutes)
        setDaylightTime(settings.turnOffAfterMinutes)
      } catch (error) {
        console.error('Failed to load settings:', error)
        // Keep default values if loading fails
      } finally {
        setIsLoading(false)
      }
    }

    loadSettings()
  }, [])

  const handleBackClick = () => {
    navigateTo(Activities.main)
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

  const handleSave = async () => {
    if (!alarmTime) {
      console.error('No alarm time set')
      return
    }

    try {
      setIsSaving(true)
      
      // Convert dayjs time to minutes since midnight
      const alarmTimeInMinutes = alarmTime.hour() * 60 + alarmTime.minute()
      
      const settings: IUserSettings = {
        AlarmTimeInMinutesSinceMidnight: alarmTimeInMinutes,
        enabledDaysOfWeek: selectedDays,
        transitionMinutes: transitionLength,
        turnOffAfterMinutes: daylightTime
      }
      
      console.log('Saving settings:', settings)
      await settingsClient.updateUserSettings(settings)
      
      // Navigate back to main activity on successful save
      navigateTo(Activities.main)
    } catch (error) {
      console.error('Failed to save settings:', error)
      // TODO: Show error message to user
    } finally {
      setIsSaving(false)
    }
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
        {isLoading ? (
          <div className="loading-container" style={{ textAlign: 'center', padding: '2rem' }}>
            <div style={{ fontSize: '2rem', marginBottom: '1rem' }}>⏳</div>
            <div>Loading settings...</div>
          </div>
        ) : (
          <>
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
          </>
        )}
      </div>
      
      <div className="action-buttons">
        <button className="cancel-button" onClick={handleBackClick} disabled={isSaving}>
          <span className="button-icon">✕</span>
          <span className="button-text">Cancel</span>
        </button>
        <button className="save-button" onClick={handleSave} disabled={isSaving || isLoading}>
          <span className="button-icon">{isSaving ? '⏳' : '💾'}</span>
          <span className="button-text">{isSaving ? 'Saving...' : 'Save'}</span>
        </button>
      </div>
    </div>
  )
}

export default SettingsActivity