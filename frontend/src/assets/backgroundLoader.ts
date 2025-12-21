/**
 * Background image loader that supports custom device-specific backgrounds
 * 
 * Priority:
 * 1. Custom background (background-custom.jpg) - device-specific, not in git
 * 2. Default background (background.jpg) - committed to repository
 * 
 * The custom background can be mounted as a volume or copied to the device.
 */

import defaultBackground from './background.jpg';

// Dynamically import custom background if it exists
// Using import.meta.glob to let Vite know about potential custom backgrounds
const backgrounds = import.meta.glob<{ default: string }>('./background-custom.{jpg,jpeg,png,webp}', { 
  eager: true,
  import: 'default'
});

// Get the first custom background if it exists
const customBackground = Object.values(backgrounds)[0];

// Export the appropriate background
const backgroundImage = customBackground || defaultBackground;

export default backgroundImage;
