import React, { useEffect, useMemo } from 'react'
import './ScreenSaverActivity.css'
import { useViewController, Activities } from '../../contexts'
import { useCurrentTime } from '../../hooks/useStateSummary'
import { isARealError } from '../../clients/StateClient'

export const ScreenSaverActivity: React.FC = () => {
  const { navigateTo } = useViewController()
  const handleBackClick = () => {
    navigateTo(Activities.main)
  }

  return (
    <div className="screensaver-activity" onClick={handleBackClick}>
      <div className="screensaver-content" style={{ width: "100%", height: "100%" }}>
        <ScreensaverClock />
      </div>
    </div>
  )
}


const ScreensaverClock: React.FC = () => { 
  const { data, isLoading, isError, error } = useCurrentTime(1000)
  const [position, setPosition] = React.useState({ 
    xPos: Math.random() * 40 + 10, 
    yPos: Math.random() * 40 + 10 
  })

  const formatTime = (date: Date) => {
    const hours = date.getHours()
    const minutes = date.getMinutes()
    const isPM = hours >= 12
    const displayHours = hours === 0 ? 12 : hours > 12 ? hours - 12 : hours
    const displayMinutes = minutes.toString().padStart(2, '0')
    
    return `${displayHours}:${displayMinutes} ${isPM ? 'PM' : 'AM'}`
  }

  let timeString: string

  if (isLoading || isARealError(isError, error) || !data) {
    timeString = formatTime(new Date())
  } else {
    timeString = formatTime(data)
  }

  useEffect(() => {
    // Update position when the time string changes (every minute)
    if (timeString && timeString.includes("00")) {
      setPosition({
        xPos: Math.random() * 40 + 10,
        yPos: Math.random() * 40 + 10
      })
    }
  }, [timeString])

  return (
      <div className="screensaver-clock" style={{ position: 'absolute', left: `${position.xPos}%`, top: `${position.yPos}%`, width: "50%", height: "50%" }}>
        {timeString}
      </div>
  )
}


export default ScreenSaverActivity