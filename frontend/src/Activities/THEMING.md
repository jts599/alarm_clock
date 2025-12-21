# Background-Based Theming

## Overview

The application automatically adjusts its styling based on which background image is active (default or custom).

## How It Works

1. **Background Loader** (`src/assets/backgroundLoader.ts`) detects which background is active at build time
2. **CSS Variables** are set based on the background type using CSS classes
3. **Components** automatically apply the appropriate class: `bg-default` or `bg-custom`

## CSS Variables

Each background type can define its own values for:

```css
--text-shadow: /* Shadow for text readability */
--component-backdrop: /* Backdrop color for component containers */
```

### Default Background Theme

```css
.main-activity.bg-default,
.settings-activity.bg-default {
  --text-shadow: 2px 2px 8px rgba(0, 0, 0, 0.8);
  --component-backdrop: rgba(0, 0, 0, 0.3);
}
```

### Custom Background Theme

```css
.main-activity.bg-custom,
.settings-activity.bg-custom {
  --text-shadow: 2px 2px 12px rgba(0, 0, 0, 0.9);
  --component-backdrop: rgba(0, 0, 0, 0.4);
}
```

## Usage in Components

Components can reference these CSS variables:

```css
.my-component {
  text-shadow: var(--text-shadow);
  background: var(--component-backdrop);
}
```

## Customization

To adjust theming for better contrast with your custom background:

1. Edit the `.bg-custom` section in:

   - `src/Activities/Main/MainActivity.css`
   - `src/Activities/Settings/SettingsActivity.css`

2. Adjust values like:

   - Text shadows
   - Background opacities
   - Border colors
   - Any other styling that needs to adapt

3. Rebuild the frontend

## Adding New Variables

To add new themeable properties:

1. Add the CSS variable to both `.bg-default` and `.bg-custom` sections
2. Use the variable in your component styles
3. Adjust values as needed for each theme
