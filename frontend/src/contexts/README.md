# View Controller

A centralized navigation system for the Alarm Clock app using React Context.

## Overview

The View Controller provides a clean, accessible way to manage navigation between different views (Activities) in the application. It uses React Context to make navigation functions available to any component in the tree.

## Features

- **Centralized State**: Single source of truth for the current view
- **Global Access**: Available from any component without prop drilling
- **Type-Safe**: Full TypeScript support with type definitions
- **Convenience Methods**: Helper functions for common navigation patterns
- **Backward Compatible**: Existing `IActivityProps` interface still works

## Setup

The View Controller is already set up in `App.tsx`:

```tsx
import { ViewProvider, Activities } from "./contexts";

function App() {
  return (
    <ViewProvider initialView={Activities.main}>
      <AppContent />
    </ViewProvider>
  );
}
```

## Usage

### Import the Hook

```tsx
import { useViewController, Activities } from "./contexts";
```

### Access Navigation Functions

```tsx
function MyComponent() {
  const { currentView, navigateTo, goToMain, goToSettings } =
    useViewController();

  return (
    <div>
      <button onClick={goToSettings}>Settings</button>
      <p>Current: {Activities[currentView]}</p>
    </div>
  );
}
```

## API Reference

### `useViewController()`

Returns a `ViewContextType` object with the following properties:

#### Properties

- **`currentView: Activities`** - The currently active view/activity

#### Methods

- **`navigateTo(view: Activities): void`** - Navigate to any view
- **`goToMain(): void`** - Navigate to main view (convenience method)
- **`goToSettings(): void`** - Navigate to settings view (convenience method)
- **`goToScreenSaver(): void`** - Navigate to screensaver view (convenience method)

### `Activities` Enum

```tsx
enum Activities {
  main,
  settings,
  screensaver,
}
```

## Examples

### Basic Navigation

```tsx
function NavigationButton() {
  const { goToSettings } = useViewController();

  return <button onClick={goToSettings}>Open Settings</button>;
}
```

### Conditional Rendering

```tsx
function ConditionalComponent() {
  const { currentView } = useViewController();

  return (
    <div>
      {currentView === Activities.main && <MainContent />}
      {currentView === Activities.settings && <SettingsContent />}
    </div>
  );
}
```

### Navigation with Logic

```tsx
function SaveButton() {
  const { goToMain } = useViewController();

  const handleSave = async () => {
    await saveData();
    goToMain(); // Return to main after saving
  };

  return <button onClick={handleSave}>Save & Exit</button>;
}
```

### Toggle Between Views

```tsx
function ToggleButton() {
  const { currentView, navigateTo } = useViewController();

  const toggle = () => {
    const nextView =
      currentView === Activities.main ? Activities.settings : Activities.main;
    navigateTo(nextView);
  };

  return <button onClick={toggle}>Toggle View</button>;
}
```

## Migration Guide

### Old Approach (Prop Drilling)

```tsx
// Parent component
const [activeActivity, setActiveActivity] = useState(Activities.main)
<ChildComponent activeActivitySetter={setActiveActivity} />

// Child component
interface Props {
    activeActivitySetter: React.Dispatch<React.SetStateAction<Activities>>
}
function ChildComponent({ activeActivitySetter }: Props) {
    return <button onClick={() => activeActivitySetter(Activities.settings)}>
        Settings
    </button>
}
```

### New Approach (View Controller)

```tsx
// No props needed!
function ChildComponent() {
  const { goToSettings } = useViewController();

  return <button onClick={goToSettings}>Settings</button>;
}
```

## Files

- **`contexts/ViewContext.tsx`** - Main context implementation
- **`contexts/index.ts`** - Barrel exports
- **`contexts/ViewControllerExamples.tsx`** - Usage examples
- **`App.tsx`** - Integration with the app

## Benefits

1. **No Prop Drilling**: Access navigation from any component depth
2. **Cleaner Code**: No need to pass `activeActivitySetter` through multiple levels
3. **Easier Testing**: Mock the context provider in tests
4. **Better Developer Experience**: Auto-complete for navigation methods
5. **Maintainable**: Single place to add new views or navigation logic

## Notes

- The `IActivityProps` interface is kept for backward compatibility but `activeActivitySetter` is now optional
- Existing components can be gradually migrated to use the view controller
- The view controller hook will throw an error if used outside the `ViewProvider`
