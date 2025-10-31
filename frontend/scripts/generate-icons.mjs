#!/usr/bin/env node

import fs from 'fs'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

// Paths
const iconsDir = path.join(__dirname, '../src/assets/icons')
const registryPath = path.join(__dirname, '../src/Components/Icon/iconRegistry.ts')

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
      const namespace = dir === '.' ? '' : dir.replace(/[/\\]/g, '')
      const componentName = (namespace ? toPascalCase(namespace) : '') + toPascalCase(iconName) + 'Icon'
      const relativePath = `../../assets/icons/${file.replace(/\\/g, '/')}?react`
      return `import ${componentName} from '${relativePath}'`
    }).join('\n')

    // Group files by namespace
    const namespaces = {}
    const rootIcons = []

    svgFiles.forEach(file => {
      const iconName = path.basename(file, '.svg')
      const dir = path.dirname(file)
      
      if (dir === '.') {
        // Root level icon
        rootIcons.push({ iconName, file })
      } else {
        // Namespaced icon
        const namespace = dir.replace(/[/\\]/g, '')
        if (!namespaces[namespace]) {
          namespaces[namespace] = []
        }
        namespaces[namespace].push({ iconName, file })
      }
    })

    // Generate registry object
    let registryEntries = []
    
    // Add root level icons
    rootIcons.forEach(({ iconName, file }) => {
      const componentName = toPascalCase(iconName) + 'Icon'
      registryEntries.push(`  '${iconName}': ${componentName},`)
    })

    // Add namespaced icons
    Object.entries(namespaces).forEach(([namespace, icons]) => {
      const namespaceEntries = icons.map(({ iconName, file }) => {
        const componentName = toPascalCase(namespace) + toPascalCase(iconName) + 'Icon'
        return `    '${iconName}': ${componentName},`
      }).join('\n')
      
      registryEntries.push(`  '${namespace}': {\n${namespaceEntries}\n  },`)
    })

    // Generate enum structure
    let enumEntries = []
    
    // Add root level enum entries
    rootIcons.forEach(({ iconName }) => {
      const enumKey = iconName.toUpperCase().replace(/-/g, '_')
      enumEntries.push(`  ${enumKey}: '${iconName}',`)
    })

    // Add namespaced enum entries
    Object.entries(namespaces).forEach(([namespace, icons]) => {
      const namespaceEnumName = namespace.charAt(0).toUpperCase() + namespace.slice(1)
      const namespaceEnumEntries = icons.map(({ iconName }) => {
        const enumKey = iconName.toUpperCase().replace(/-/g, '_')
        return `    ${enumKey}: '${namespace}.${iconName}',`
      }).join('\n')
      
      enumEntries.push(`  ${namespaceEnumName}: {\n${namespaceEnumEntries}\n  },`)
    })

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

  } catch (error) {
    console.error('Error generating icon registry:', error)
    process.exit(1)
  }
}

generateIconRegistry()