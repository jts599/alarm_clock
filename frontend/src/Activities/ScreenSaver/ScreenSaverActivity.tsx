import React from 'react'
import './ScreenSaverActivity.css'
import { useViewController, Activities } from '../../contexts'

export const ScreenSaverActivity: React.FC = () => {
  const { navigateTo } = useViewController()
  const handleBackClick = () => {
    navigateTo(Activities.main)
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