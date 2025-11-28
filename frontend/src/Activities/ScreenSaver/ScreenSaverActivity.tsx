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

const randomPosition = () => {
  return {
    x: Math.random() * 20 + 5, // between 5% and 25%
    y: Math.random() * 20 + 5  // between 5% and 25%
  }
}

const ScreensaverClock: React.FC = () => { 
  const { data, isLoading, isError, error } = useCurrentTime(1000)
  const initialPosition = useMemo(() => randomPosition(), [])
  const [position, setPosition] = React.useState({ 
    xPos: initialPosition.x, 
    yPos: initialPosition.y 
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
      const newPosition = randomPosition()
      setPosition({
        xPos: newPosition.x,
        yPos: newPosition.y
      })
    }
  }, [timeString])

  return (
      <div className="screensaver-clock" style={{ position: 'absolute', left: `${position.xPos}%`, top: `${position.yPos}%`, width: "75%", height: "75%" }}>
        {timeString}
      </div>
  )
}


export default ScreenSaverActivity