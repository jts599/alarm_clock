#!/usr/bin/env node

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

// Paths
const iconsDir = path.join(__dirname, '../src/assets/icons')
const registryPath = path.join(__dirname, '../src/Components/Icon/iconRegistry.ts')
const weatherCsvPath = path.join(__dirname, '../src/assets/icons/weather/weather_icons_descriptions.csv')
const sharedWeatherIconsPath = path.join(__dirname, '../../shared/weather/valid_weather_icons.txt')

// Helper function to convert filename to camelCase
function toCamelCase(str) {
  return str.replace(/-([a-z])/g, (g) => g[1].toUpperCase())
}

// Helper function to convert filename to PascalCase for component names
function toPascalCase(str) {
  const camelCase = toCamelCase(str)
  return camelCase.charAt(0).toUpperCase() + camelCase.slice(1)
}

// Helper function to recursively find all SVG files
function findSvgFiles(dir, baseDir = dir) {
  const files = []
  const items = fs.readdirSync(dir)
  
  for (const item of items) {
    const fullPath = path.join(dir, item)
    const stat = fs.statSync(fullPath)
    
    if (stat.isDirectory()) {
      files.push(...findSvgFiles(fullPath, baseDir))
    } else if (item.endsWith('.svg')) {
      const relativePath = path.relative(baseDir, fullPath)
      files.push(relativePath)
    }
  }
  
  return files
}

// Helper function to create icon key from file path
function createIconKey(file) {
  const iconName = path.basename(file, '.svg')
  const dir = path.dirname(file)
  
  if (dir === '.') {
    return iconName
  } else {
    const pathParts = dir.split(/[/\\]/)
    return pathParts.join('.') + '.' + iconName
  }
}

// Function to generate weather icons validation file for backend
function generateWeatherIconsValidation(svgFiles) {
  try {
    // Check if weather CSV exists
    if (!fs.existsSync(weatherCsvPath)) {
      console.log(`Weather CSV not found: ${weatherCsvPath}`)
      return
    }

    // Read and parse the CSV file
    const csvContent = fs.readFileSync(weatherCsvPath, 'utf8')
    const csvLines = csvContent.trim().split('\n')
    
    // Create a set of available icon keys from SVG files
    const availableIconKeys = new Set()
    svgFiles.forEach(file => {
      const iconKey = createIconKey(file)
      availableIconKeys.add(iconKey)
    })

    // Process CSV and match with available icons
    const validWeatherIcons = []
    
    csvLines.forEach(line => {
      // Parse CSV line - handle quoted descriptions that may contain commas
      const match = line.match(/^"?([^",]+)"?,?"?([^"]*)"?$/)
      if (!match) {
        console.warn(`Skipping malformed CSV line: ${line}`)
        return
      }
      
      const iconKey = match[1].trim()
      const description = match[2].trim()
      
      // Check if this icon exists in our generated icons
      if (availableIconKeys.has(iconKey)) {
        validWeatherIcons.push(`${iconKey}|${description}`)
      } else {
        console.warn(`Weather icon not found in SVG files: ${iconKey}`)
      }
    })

    // Write the validation file to shared directory for backend access
    const validationContent = validWeatherIcons.join('\n') + '\n'
    const sharedWeatherDir = path.dirname(sharedWeatherIconsPath)
    if (!fs.existsSync(sharedWeatherDir)) {
      fs.mkdirSync(sharedWeatherDir, { recursive: true })
    }
    fs.writeFileSync(sharedWeatherIconsPath, validationContent, 'utf8')
    
    console.log(`✅ Generated weather icons validation file with ${validWeatherIcons.length} icons`)
    console.log(`   Written to: ${sharedWeatherIconsPath}`)
    
  } catch (error) {
    console.error('Error generating weather icons validation:', error)
  }
}

function generateIconRegistry() {
  try {
    // Check if icons directory exists
    if (!fs.existsSync(iconsDir)) {
      console.error(`Icons directory not found: ${iconsDir}`)
      process.exit(1)
    }

    // Find all SVG files recursively
    const svgFiles = findSvgFiles(iconsDir)

    if (svgFiles.length === 0) {
      console.log('No SVG files found in icons directory')
      return
    }

    // Generate imports
    const imports = svgFiles.map(file => {
      const iconName = path.basename(file, '.svg')
      const dir = path.dirname(file)
      const pathParts = dir === '.' ? [] : dir.split(/[/\\]/)
      const componentName = pathParts.map(part => toPascalCase(part)).join('') + toPascalCase(iconName) + 'Icon'
      const relativePath = `../../assets/icons/${file.replace(/\\/g, '/')}?react`
      return `import ${componentName} from '${relativePath}'`
    }).join('\n')

    // Group files by namespace hierarchy
    const namespaceTree = {}
    const rootIcons = []

    svgFiles.forEach(file => {
      const iconName = path.basename(file, '.svg')
      const dir = path.dirname(file)
      
      if (dir === '.') {
        // Root level icon
        rootIcons.push({ iconName, file })
      } else {
        // Namespaced icon - handle nested directories properly
        const pathParts = dir.split(/[/\\]/)
        let currentLevel = namespaceTree
        
        // Navigate to the correct nested level
        pathParts.forEach(part => {
          if (!currentLevel[part]) {
            currentLevel[part] = { _icons: [], _children: {} }
          }
          currentLevel = currentLevel[part]._children
        })
        
        // Add icon to the final level
        const finalNamespace = pathParts[pathParts.length - 1]
        if (!namespaceTree[pathParts[0]]) {
          namespaceTree[pathParts[0]] = { _icons: [], _children: {} }
        }
        
        let targetLevel = namespaceTree
        pathParts.forEach(part => {
          if (!targetLevel[part]) {
            targetLevel[part] = { _icons: [], _children: {} }
          }
          if (part === pathParts[pathParts.length - 1]) {
            targetLevel[part]._icons.push({ iconName, file, pathParts })
          } else {
            targetLevel = targetLevel[part]._children
          }
        })
      }
    })

    // Generate registry object with proper nesting
    function generateRegistryLevel(tree, level = 0) {
      const entries = []
      const indent = '  '.repeat(level + 1)
      
      Object.entries(tree).forEach(([key, value]) => {
        if (value._icons && value._icons.length > 0) {
          // This namespace has icons
          const iconEntries = value._icons.map(({ iconName, pathParts }) => {
            const componentName = pathParts.map(part => toPascalCase(part)).join('') + toPascalCase(iconName) + 'Icon'
            return `${indent}  '${iconName}': ${componentName},`
          }).join('\n')
          
          if (Object.keys(value._children).length > 0) {
            // Has both icons and children
            const childEntries = generateRegistryLevel(value._children, level + 1)
            entries.push(`${indent}'${key}': {\n${iconEntries}\n${childEntries}\n${indent}},`)
          } else {
            // Only has icons
            entries.push(`${indent}'${key}': {\n${iconEntries}\n${indent}},`)
          }
        } else if (Object.keys(value._children).length > 0) {
          // Only has children, no icons at this level
          const childEntries = generateRegistryLevel(value._children, level + 1)
          entries.push(`${indent}'${key}': {\n${childEntries}\n${indent}},`)
        }
      })
      
      return entries.join('\n')
    }

    let registryEntries = []
    
    // Add root level icons
    rootIcons.forEach(({ iconName, file }) => {
      const componentName = toPascalCase(iconName) + 'Icon'
      registryEntries.push(`  '${iconName}': ${componentName},`)
    })

    // Add namespaced entries
    const namespacedEntries = generateRegistryLevel(namespaceTree)
    if (namespacedEntries) {
      registryEntries.push(namespacedEntries)
    }

    // Generate enum structure with proper nesting
    function generateEnumLevel(tree, pathPrefix = '', level = 0) {
      const entries = []
      const indent = '  '.repeat(level + 1)
      
      Object.entries(tree).forEach(([key, value]) => {
        const currentPath = pathPrefix ? `${pathPrefix}.${key}` : key
        
        if (value._icons && value._icons.length > 0) {
          // This namespace has icons
          const iconEntries = value._icons.map(({ iconName }) => {
            const enumKey = iconName.toUpperCase().replace(/-/g, '_')
            return `${indent}  ${enumKey}: '${currentPath}.${iconName}',`
          }).join('\n')
          
          if (Object.keys(value._children).length > 0) {
            // Has both icons and children
            const childEntries = generateEnumLevel(value._children, currentPath, level + 1)
            const capitalizedKey = key.charAt(0).toUpperCase() + key.slice(1)
            entries.push(`${indent}${capitalizedKey}: {\n${iconEntries}\n${childEntries}\n${indent}},`)
          } else {
            // Only has icons
            const capitalizedKey = key.charAt(0).toUpperCase() + key.slice(1)
            entries.push(`${indent}${capitalizedKey}: {\n${iconEntries}\n${indent}},`)
          }
        } else if (Object.keys(value._children).length > 0) {
          // Only has children, no icons at this level
          const childEntries = generateEnumLevel(value._children, currentPath, level + 1)
          const capitalizedKey = key.charAt(0).toUpperCase() + key.slice(1)
          entries.push(`${indent}${capitalizedKey}: {\n${childEntries}\n${indent}},`)
        }
      })
      
      return entries.join('\n')
    }

    let enumEntries = []
    
    // Add root level enum entries
    rootIcons.forEach(({ iconName }) => {
      const enumKey = iconName.toUpperCase().replace(/-/g, '_')
      enumEntries.push(`  ${enumKey}: '${iconName}',`)
    })

    // Add namespaced enum entries
    const namespacedEnumEntries = generateEnumLevel(namespaceTree)
    if (namespacedEnumEntries) {
      enumEntries.push(namespacedEnumEntries)
    }

    // Generate the complete registry file
    const registryContent = `// This file is auto-generated. Do not edit manually.
// Run 'npm run generate-icons' to regenerate.

${imports}

// Icon registry mapping icon names to components
export const iconRegistry = {
${registryEntries.join('\n')}
} as const

// Enum for typesafe icon name references
export const Icons = {
${enumEntries.join('\n')}
} as const

// Type for valid icon names
export type IconName = keyof typeof iconRegistry | \`\${keyof typeof iconRegistry[keyof typeof iconRegistry]}\`
`

    // Write the registry file
    fs.writeFileSync(registryPath, registryContent, 'utf8')

    console.log(`✅ Generated icon registry with ${svgFiles.length} icons:`)
    svgFiles.forEach(file => {
      const iconName = path.basename(file, '.svg')
      console.log(`   - ${iconName}`)
    })

    // Generate weather icons validation file for backend
    generateWeatherIconsValidation(svgFiles)

  } catch (error) {
    console.error('Error generating icon registry:', error)
    process.exit(1)
  }
}

generateIconRegistry()