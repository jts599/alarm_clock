import React, { useState, useRef, useEffect } from 'react'
import './Tooltip.css'

interface TooltipProps {
  text: string
  children: React.ReactNode
  position?: 'top' | 'bottom' | 'left' | 'right'
  delay?: number
}

export const Tooltip: React.FC<TooltipProps> = ({ 
  text, 
  children, 
  position = 'top',
  delay = 500 
}) => {
  const [isVisible, setIsVisible] = useState(false)
  const [isMobile, setIsMobile] = useState(false)
  const timeoutRef = useRef<number>()
  const tooltipRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    // Check if device is mobile
    const checkMobile = () => {
      setIsMobile(window.innerWidth <= 768 || 'ontouchstart' in window)
    }
    
    checkMobile()
    window.addEventListener('resize', checkMobile)
    return () => window.removeEventListener('resize', checkMobile)
  }, [])

  const showTooltip = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }
    timeoutRef.current = window.setTimeout(() => {
      setIsVisible(true)
    }, delay)
  }

  const hideTooltip = () => {
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current)
    }
    setIsVisible(false)
  }

  const handleClick = () => {
    if (isMobile) {
      setIsVisible(!isVisible)
    }
  }

  const handleMouseEnter = () => {
    if (!isMobile) {
      showTooltip()
    }
  }

  const handleMouseLeave = () => {
    if (!isMobile) {
      hideTooltip()
    }
  }

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (isMobile && isVisible && tooltipRef.current && !tooltipRef.current.contains(event.target as Node)) {
        setIsVisible(false)
      }
    }

    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [isMobile, isVisible])

  return (
    <div 
      className="tooltip-container"
      ref={tooltipRef}
      onMouseEnter={handleMouseEnter}
      onMouseLeave={handleMouseLeave}
      onClick={handleClick}
    >
      {children}
      {isVisible && (
        <div className={`tooltip-content tooltip-${position}`}>
          <div className="tooltip-text">{text}</div>
          <div className={`tooltip-arrow tooltip-arrow-${position}`}></div>
        </div>
      )}
    </div>
  )
}

export default Tooltip