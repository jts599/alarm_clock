import React from 'react'
import { iconRegistry } from './iconRegistry'

export interface ISize {
  width: number | string
  height: number | string
}

export interface IconProps {
  name: string // Now accepts both 'iconName' and 'namespace.iconName'
  size?: number | string | ISize
  className?: string
  color?: string
  onClick?: () => void
}

// Type guard to check if something is a React component
function isReactComponent(component: any): component is React.ComponentType<any> {
  return typeof component === 'function'
}

export const Icon: React.FC<IconProps> = ({ 
  name, 
  size = 24, 
  className = '', 
  color = 'currentColor',
  onClick 
}) => {
  // Handle namespaced icons with arbitrary depth (e.g., 'weather.day.sunny')
  const getIconComponent = (iconName: string): React.ComponentType<any> | null => {
    if (iconName.includes('.')) {
      const parts = iconName.split('.')
      let current: any = iconRegistry
      
      // Navigate through the nested structure
      for (const part of parts) {
        if (current && typeof current === 'object' && part in current) {
          current = current[part]
        } else {
          return null
        }
      }
      
      return isReactComponent(current) ? current : null
    }
    
    // Handle root level icons
    const rootIcon = iconRegistry[iconName as keyof typeof iconRegistry]
    return isReactComponent(rootIcon) ? rootIcon : null
  }

  const IconComponent = getIconComponent(name)
  
  if (!IconComponent) {
    console.warn(`Icon "${name}" not found in registry`)
    return null
  }

  const calculatedSize = calculateSize(size)
  const sizeStyle = typeof size === 'number' ? `${size}px` : size

  return (
    <IconComponent
      width={calculatedSize.width}
      height={calculatedSize.height}
      className={className}
      style={{ color, display: 'block' }}
      onClick={onClick}
    />
  )
}

function calculateSize(size: number | string | ISize): ISize {
  if (typeof size === 'number') {
    return { width: size, height: size }
  } else if (typeof size === 'string') {
    return { width: size, height: size }
  } else {
    return size
  }
}

export default Icon