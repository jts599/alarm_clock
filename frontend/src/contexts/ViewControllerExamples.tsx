/**
 * View Controller Usage Examples
 * 
 * This file demonstrates how to use the View Controller in any component
 */

import { useViewController, Activities } from '../contexts'

// Example 1: Basic navigation
export function ExampleComponent1() {
    const { currentView, navigateTo } = useViewController()
    
    return (
        <div>
            <p>Current view: {Activities[currentView]}</p>
            <button onClick={() => navigateTo(Activities.settings)}>
                Go to Settings
            </button>
        </div>
    )
}

// Example 2: Navigation buttons
export function ExampleComponent2() {
    const { navigateTo } = useViewController()
    
    return (
        <div>
            <button onClick={() => navigateTo(Activities.main)}>Home</button>
            <button onClick={() => navigateTo(Activities.settings)}>Settings</button>
            <button onClick={() => navigateTo(Activities.screensaver)}>Screen Saver</button>
        </div>
    )
}

// Example 3: Conditional rendering based on current view
export function ExampleComponent3() {
    const { currentView } = useViewController()
    
    return (
        <div>
            {currentView === Activities.main && <p>You're on the main screen</p>}
            {currentView === Activities.settings && <p>You're in settings</p>}
        </div>
    )
}

// Example 4: Navigation with logic
export function ExampleComponent4() {
    const { navigateTo, currentView } = useViewController()
    
    const handleSave = () => {
        // Save logic here...
        
        // Navigate back to main after saving
        navigateTo(Activities.main)
    }
    
    const toggleView = () => {
        const nextView = currentView === Activities.main 
            ? Activities.settings 
            : Activities.main
        navigateTo(nextView)
    }
    
    return (
        <div>
            <button onClick={handleSave}>Save & Return</button>
            <button onClick={toggleView}>Toggle View</button>
        </div>
    )
}

// Example 5: Using view state
interface SettingsViewState {
    brightness: number
    volume: number
    isDirty: boolean
}

export function ExampleComponent5() {
    const { getViewState, setViewState } = useViewController()
    
    const state = getViewState<SettingsViewState>(Activities.settings)
    
    const handleBrightnessChange = (value: number) => {
        setViewState<SettingsViewState>(Activities.settings, {
            brightness: value,
            isDirty: true
        })
    }
    
    return (
        <div>
            <input 
                type="range" 
                value={state.brightness || 50} 
                onChange={(e) => handleBrightnessChange(Number(e.target.value))}
            />
            <p>Brightness: {state.brightness || 50}</p>
            {state.isDirty && <p>Unsaved changes</p>}
        </div>
    )
}

// Example 6: Clearing view state
export function ExampleComponent6() {
    const { clearViewState, clearAllViewStates } = useViewController()
    
    const handleReset = () => {
        clearViewState(Activities.settings)
    }
    
    const handleResetAll = () => {
        clearAllViewStates()
    }
    
    return (
        <div>
            <button onClick={handleReset}>Reset Settings State</button>
            <button onClick={handleResetAll}>Reset All View States</button>
        </div>
    )
}

// Example 7: Preserving state across navigation
interface FormState {
    name: string
    email: string
}

export function ExampleComponent7() {
    const { currentView, navigateTo, getViewState, setViewState } = useViewController()
    
    const formState = getViewState<FormState>(Activities.settings)
    
    const handleInputChange = (field: keyof FormState, value: string) => {
        setViewState<FormState>(Activities.settings, {
            [field]: value
        })
    }
    
    const handleNavigateAway = () => {
        // State is preserved automatically!
        navigateTo(Activities.main)
    }
    
    return (
        <div>
            <input 
                value={formState.name || ''} 
                onChange={(e) => handleInputChange('name', e.target.value)}
                placeholder="Name"
            />
            <input 
                value={formState.email || ''} 
                onChange={(e) => handleInputChange('email', e.target.value)}
                placeholder="Email"
            />
            <button onClick={handleNavigateAway}>
                Go to Main (state preserved)
            </button>
        </div>
    )
}
