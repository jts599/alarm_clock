import React from 'react'
import WeatherComponent from '../../Components/WeatherComponent'
import NextEventComponent from '../../Components/NextEventComponent'
import LightStatusComponent from '../../Components/LightStatusComponent'
import TimeComponent from '../../Components/TimeComponent'
import LightControlComponent from '../../Components/LightControlComponent'
import { IActivityProps } from '../../App'
import './MainActivity.css'

export const MainActivity: React.FC<IActivityProps> = ({ activeActivitySetter }) => {
  return (
    <div className="main-activity">
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
          <LightControlComponent activeActivitySetter={activeActivitySetter} />
        </div>
      </div>
    </div>
  )
}

export default MainActivity
