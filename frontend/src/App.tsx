import React from 'react'
import MainActivity from './Activities/Main/MainActivity'
import SettingsActivity from './Activities/Settings/SettingsActivity'
import ScreenSaverActivity from './Activities/ScreenSaver/ScreenSaverActivity'
import { ViewProvider, useViewController, Activities } from './contexts/ViewContext'

// Keep the interface for backward compatibility with existing Activities
export interface IActivityProps {
    activeActivitySetter?: React.Dispatch<React.SetStateAction<Activities>>
}

function AppContent() {
    const { currentView } = useViewController()

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
