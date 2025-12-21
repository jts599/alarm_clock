import React, { useEffect, useState } from 'react'
import WeatherComponent from '../../Components/WeatherComponent'
import NextEventComponent from '../../Components/NextEventComponent'
import LightStatusComponent from '../../Components/LightStatusComponent'
import TimeComponent from '../../Components/TimeComponent'
import LightControlComponent from '../../Components/LightControlComponent'
import defaultBackground from '../../assets/background.jpg'
import { getBackgroundImage, BackgroundType } from '../../assets/backgroundLoader'
import './MainActivity.css'

export const MainActivity: React.FC = () => {
  const [backgroundImage, setBackgroundImage] = useState<string>(defaultBackground)
  const [backgroundType, setBackgroundType] = useState<BackgroundType>(BackgroundType.DEFAULT)
  
  useEffect(() => {
    // Detect background at runtime via backend API
    getBackgroundImage().then(({ type, image }) => {
      setBackgroundType(type)
      setBackgroundImage(image)
    })
  }, [])
  
  const bgClass = `main-activity bg-${backgroundType}`
  
  return (
    <div className={bgClass} style={{ backgroundImage: `url(${backgroundImage})` }}>
      {/* Main time display - center of screen */}
      <div className="time-section">
        <TimeComponent />
      </div>
      
      {/* Component grid - positioned in corners */}
      <div className="components-grid">
        <div className="light-status-container">
          <LightStatusComponent />
        </div>
        <div className="weather-container">
          <WeatherComponent />
        </div>
        <div className="next-event-container">
          <NextEventComponent />
        </div>
        <div className="light-control-container">
          <LightControlComponent />
        </div>
      </div>
    </div>
  )
}

export default MainActivity
