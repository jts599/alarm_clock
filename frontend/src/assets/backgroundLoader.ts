/**
 * Background image loader that supports custom device-specific backgrounds
 * 
 * Priority:
 * 1. Custom background - served by backend from Docker volume mount
 * 2. Default background (background.jpg) - committed to repository
 * 
 * Custom backgrounds are served through the backend API to avoid build-time detection issues.
 */

import defaultBackground from './background.jpg';

export enum BackgroundType {
    DEFAULT = 'default',
    CUSTOM = 'custom'
}

const CUSTOM_BACKGROUND_API = '/api/background/custom';

let cachedBackgroundType: BackgroundType | null = null;
let cachedBackgroundImage: string | null = null;

/**
 * Check if custom background exists via backend API
 * The backend serves the custom background from a Docker volume mount
 */
async function detectBackgroundImage(): Promise<{ type: BackgroundType; image: string }> {
    // Return cached result if available
    if (cachedBackgroundType && cachedBackgroundImage) {
        return { type: cachedBackgroundType, image: cachedBackgroundImage };
    }

    try {
        // Check if custom background exists via backend API
        const response = await fetch('/api/background/custom/exists');
        if (response.ok) {
            const data = await response.json();
            if (data.exists) {
                console.log('Custom background available via backend');
                cachedBackgroundType = BackgroundType.CUSTOM;
                cachedBackgroundImage = CUSTOM_BACKGROUND_API;
                return { type: BackgroundType.CUSTOM, image: CUSTOM_BACKGROUND_API };
            }
        }
    } catch (error) {
        console.log('Custom background not available, using default');
    }

    // Fall back to default background
    console.log('Using default background');
    cachedBackgroundType = BackgroundType.DEFAULT;
    cachedBackgroundImage = defaultBackground;
    return { type: BackgroundType.DEFAULT, image: defaultBackground };
}

/**
 * Get the configured background image
 * Returns a promise that resolves to the background type and image path
 */
export async function getBackgroundImage(): Promise<{ type: BackgroundType; image: string }> {
    return detectBackgroundImage();
}

// For backwards compatibility, export the default background synchronously
// Components should use getBackgroundImage() for proper custom background support
export const configuredBackgroundType = BackgroundType.DEFAULT;
export default defaultBackground;
