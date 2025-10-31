# Icon System with Namespacing

This project uses an automated icon generation system that creates a centralized icon registry from SVG files, with support for namespacing based on directory structure.

## Directory Structure

```
src/assets/icons/
├── alarm.svg           # Root level: Icons.ALARM
├── settings.svg        # Root level: Icons.SETTINGS
├── weather/
│   ├── sunny.svg       # Namespaced: Icons.Weather.SUNNY
│   ├── rainy.svg       # Namespaced: Icons.Weather.RAINY
│   └── cloudy.svg      # Namespaced: Icons.Weather.CLOUDY
└── lights/
    ├── lights-on.svg   # Namespaced: Icons.Lights.LIGHTS_ON
    └── lights-off.svg  # Namespaced: Icons.Lights.LIGHTS_OFF
```

## How to use icons

1. **Add SVG files** to `/src/assets/icons/` (or subdirectories for namespacing)
2. **Run the generator**: `npm run generate-icons`
3. **Use in components**:

   ```tsx
   import { Icon, Icons } from './Components/Icon'

   // Root level icons
   <Icon name="alarm" />
   <Icon name={Icons.ALARM} />

   // Namespaced icons
   <Icon name="weather.sunny" />
   <Icon name={Icons.Weather.SUNNY} />

   // With props
   <Icon name={Icons.Weather.RAINY} size={32} color="#4A90E2" />
   ```

## Icon naming and namespacing

- **Root level**: `alarm.svg` → `Icons.ALARM` or `"alarm"`
- **Namespaced**: `weather/sunny.svg` → `Icons.Weather.SUNNY` or `"weather.sunny"`
- **Multi-word**: `lights/lights-on.svg` → `Icons.Lights.LIGHTS_ON` or `"lights.lights-on"`

## Examples

```tsx
// Root level icons
<Icon name="alarm" />
<Icon name={Icons.ALARM} />

// Namespaced icons with strings
<Icon name="weather.sunny" />
<Icon name="lights.lights-on" />

// Namespaced icons with enum (typesafe)
<Icon name={Icons.Weather.SUNNY} />
<Icon name={Icons.Lights.LIGHTS_ON} />

// Custom size and color
<Icon name={Icons.Weather.RAINY} size={48} color="#4A90E2" />

// With CSS classes and click handler
<Icon
  name={Icons.Lights.LIGHTS_OFF}
  className="hover:opacity-50 cursor-pointer"
  onClick={() => toggleLights()}
/>
```

## Notes

- The `iconRegistry.ts` file is auto-generated - don't edit it manually
- TypeScript will provide autocomplete for available icon names
- Icons support all standard SVG styling via CSS
