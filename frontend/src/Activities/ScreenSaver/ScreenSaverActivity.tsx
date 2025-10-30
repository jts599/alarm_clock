import React from 'react'
import { Activities, IActivityProps } from '../../App'
import './ScreenSaverActivity.css'

export const ScreenSaverActivity: React.FC<IActivityProps> = ({ activeActivitySetter }) => {
  const handleBackClick = () => {
    activeActivitySetter(Activities.main)
  }

  return (
    <div className="screensaver-activity" onClick={handleBackClick}>
      <div className="screensaver-content">
        <p>Screen Saver Mode</p>
        <p className="screensaver-hint">Click anywhere to return</p>
      </div>
    </div>
  )
}

export default ScreenSaverActivity