import React, { createContext, useContext, useState, ReactNode } from 'react'

export enum Activities {
    main,
    settings,
    screensaver
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
type ViewState = Record<string, any>

export interface ViewContextType {
    currentView: Activities
    navigateTo: (view: Activities) => void
    getViewState: <T extends ViewState>(view: Activities) => T
    setViewState: <T extends ViewState>(view: Activities, state: Partial<T>) => void
    clearViewState: (view: Activities) => void
    clearAllViewStates: () => void
}

const ViewContext = createContext<ViewContextType | undefined>(undefined)

export interface ViewProviderProps {
    children: ReactNode
    initialView?: Activities
}

export const ViewProvider: React.FC<ViewProviderProps> = ({ 
    children, 
    initialView = Activities.main 
}) => {
    const [currentView, setCurrentView] = useState<Activities>(initialView)
    const [viewStates, setViewStates] = useState<Map<Activities, ViewState>>(new Map())

    const navigateTo = (view: Activities) => {
        setCurrentView(view)
    }

    const getViewState = <T extends ViewState>(view: Activities): T => {
        return (viewStates.get(view) || {}) as T
    }

    const setViewState = <T extends ViewState>(view: Activities, state: Partial<T>) => {
        setViewStates(prev => {
            const newMap = new Map(prev)
            const currentState = newMap.get(view) || {}
            newMap.set(view, { ...currentState, ...state })
            return newMap
        })
    }

    const clearViewState = (view: Activities) => {
        setViewStates(prev => {
            const newMap = new Map(prev)
            newMap.delete(view)
            return newMap
        })
    }

    const clearAllViewStates = () => {
        setViewStates(new Map())
    }

    const value: ViewContextType = {
        currentView,
        navigateTo,
        getViewState,
        setViewState,
        clearViewState,
        clearAllViewStates
    }

    return (
        <ViewContext.Provider value={value}>
            {children}
        </ViewContext.Provider>
    )
}

export const useViewController = (): ViewContextType => {
    const context = useContext(ViewContext)
    if (context === undefined) {
        throw new Error('useViewController must be used within a ViewProvider')
    }
    return context
}
