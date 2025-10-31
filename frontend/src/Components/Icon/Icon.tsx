import React from 'react'
import { iconRegistry } from './iconRegistry'

export interface IconProps {
  name: string // Now accepts both 'iconName' and 'namespace.iconName'
  size?: number | string
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
  // Handle namespaced icons (e.g., 'weather.sunny')
  const getIconComponent = (iconName: string): React.ComponentType<any> | null => {
    if (iconName.includes('.')) {
      const [namespace, icon] = iconName.split('.')
      const namespaceRegistry = iconRegistry[namespace as keyof typeof iconRegistry]
      if (namespaceRegistry && typeof namespaceRegistry === 'object' && !isReactComponent(namespaceRegistry)) {
        const iconComponent = (namespaceRegistry as any)[icon]
        return isReactComponent(iconComponent) ? iconComponent : null
      }
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

  const sizeStyle = typeof size === 'number' ? `${size}px` : size

  return (
    <IconComponent
      width={sizeStyle}
      height={sizeStyle}
      className={className}
      style={{ color }}
      onClick={onClick}
    />
  )
}

export default Icon