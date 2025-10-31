// Import the CSV file as text (Vite will handle this)
import csvContent from '../../assets/icons/weather/weather_icons_descriptions.csv?raw';
import { iconRegistry, Icons } from './iconRegistry';

export function getPossibleWeatherIcons(): string { 
    const lines = csvContent.split('\n');
    const icons: string[] = [];
    lines.forEach(line => {
        if (line.trim()) { // Skip empty lines
            const parsed = parseCSVLine(line);
            if (parsed.length >= 2) {
                const icon_key = parsed[0].trim();
                const iconContents = parsed[1].trim().replace(/^"(.*)"$/, '$1'); // Strip quotes
                const icon = `${icon_key}|${iconContents}`;
                icons.push(icon);
            }
        }
    });
    
    // Filter out icons that don't exist in the registry
    const filteredIcons = removeIconsThatDoNotExist(icons);
    return filteredIcons.join('\n');
}

function removeIconsThatDoNotExist(icons: string[]): string[] {
    const filteredIcons: string[] = [];
    
    // Get all available icon keys from the registry
    const availableIconKeys = getAllIconKeys();
    
    icons.forEach(icon => {
        const [iconKey] = icon.split('|');
        if (availableIconKeys.has(iconKey)) {
            filteredIcons.push(icon);
        } else {
            console.warn(`Icon not found in registry: ${iconKey}`);
        }
    });
    
    return filteredIcons;
}

interface IEnumeratedKey {
    key: string;
    value?: any;
    hasChildren: boolean;
}   

function enumerateValues(obj: any, values: string[] = []): string[] {
    let keys: IEnumeratedKey[] = [];

    Object.keys(obj).forEach(key => {
        const value = obj[key];
        const hasChildren = (typeof value === 'object' && value !== null);
        keys.push({ key: key, hasChildren: hasChildren, value: value});
    });

    keys.forEach(({ hasChildren, value }) => {
        if (hasChildren) {
            enumerateValues(value, values);
        } else {
            values.push(value);
        }
    });

    return values;
}

function getAllIconKeys(): Set<string> {
    const keys = new Set<string>();
    const iconKeys = enumerateValues(iconRegistry.weather);
    iconKeys.forEach(key => keys.add(key));
    return keys;
}

function parseCSVLine(line: string): string[] {
    const result: string[] = [];
    let current = '';
    let inQuotes = false;
    
    for (let i = 0; i < line.length; i++) {
        const char = line[i];
        
        if (char === '"') {
            inQuotes = !inQuotes;
        } else if (char === ',' && !inQuotes) {
            result.push(current);
            current = '';
        } else {
            current += char;
        }
    }
    
    // Add the last field
    result.push(current);
    
    return result;
}