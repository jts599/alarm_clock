import React, { useEffect, useRef } from 'react'
import MainActivity from './Activities/Main/MainActivity'
import SettingsActivity from './Activities/Settings/SettingsActivity'
import ScreenSaverActivity from './Activities/ScreenSaver/ScreenSaverActivity'
import { ViewProvider, useViewController, Activities } from './contexts/ViewContext'

// Keep the interface for backward compatibility with existing Activities
export interface IActivityProps {
    activeActivitySetter?: React.Dispatch<React.SetStateAction<Activities>>
}

const INACTIVITY_TIMEOUT = 30 * 1000 // 30 seconds in milliseconds

function AppContent() {
    const { currentView, navigateTo } = useViewController()
    const inactivityTimerRef = useRef<number | null>(null)

    const resetInactivityTimer = () => {
        // Clear existing timer
        if (inactivityTimerRef.current) {
            clearTimeout(inactivityTimerRef.current)
        }

        // Don't set a timer if already on screensaver
        if (currentView === Activities.screensaver) {
            return
        }

        // Set new timer
        inactivityTimerRef.current = window.setTimeout(() => {
            navigateTo(Activities.screensaver)
        }, INACTIVITY_TIMEOUT)
    }

    useEffect(() => {
        // Reset timer when view changes
        resetInactivityTimer()

        // Set up event listeners for user activity
        const events = ['mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click']
        
        events.forEach(event => {
            document.addEventListener(event, resetInactivityTimer, true)
        })

        // Cleanup
        return () => {
            if (inactivityTimerRef.current) {
                clearTimeout(inactivityTimerRef.current)
            }
            events.forEach(event => {
                document.removeEventListener(event, resetInactivityTimer, true)
            })
        }
    }, [currentView, navigateTo])

    const renderActivity = () => {
        switch (currentView) {
            case Activities.main:
                return <MainActivity  />
            case Activities.settings:
                return <SettingsActivity />
            case Activities.screensaver:
                return <ScreenSaverActivity  />
            default:
                return <MainActivity />
        }
    }

    return (
        <div style={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            minHeight: '100vh',
            backgroundColor: '#1a1a1a',
            boxSizing: 'border-box'
        }}>
            <div style={{
                width: '1280px',
                height: '720px',
                backgroundColor: '#2d2d2d',
                boxShadow: '0 4px 12px rgba(0, 0, 0, 0.3)',
                boxSizing: 'border-box',
                display: 'flex',
                flexDirection: 'column'
            }}>
                {renderActivity()}
            </div>
        </div>
    )
}

export default function App() {
    return (
        <ViewProvider initialView={Activities.main}>
            <AppContent />
        </ViewProvider>
    )
}
