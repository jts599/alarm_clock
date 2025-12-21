# Background Images

## Overview

The application supports both a default background image and a device-specific custom background.

## Files

- **`background.jpg`** - Default background image (committed to git)
- **`background-custom.jpg`** - Custom background image (ignored by git, device-specific)
- **`backgroundLoader.ts`** - Dynamic loader that prioritizes custom over default

## Using a Custom Background

To use a custom background on a device:

1. Place your custom image at: `frontend/src/assets/background-custom.jpg`
2. The application will automatically use it instead of the default

### Docker Volume Mount

To mount a custom background in Docker, add a volume mount:

```yaml
volumes:
  - /path/on/host/my-background.jpg:/app/src/assets/background-custom.jpg:ro
```

Or with docker-compose:

```yaml
services:
  frontend:
    volumes:
      - ./custom-background.jpg:/app/src/assets/background-custom.jpg:ro
```

### Direct Copy

For non-containerized deployments:

```bash
cp /path/to/your/image.jpg frontend/src/assets/background-custom.jpg
```

## Supported Formats

The custom background can be any of these formats (update the filename accordingly):

- `background-custom.jpg`
- `background-custom.jpeg`
- `background-custom.png`
- `background-custom.webp`

All custom formats are ignored by git.

## Fallback Behavior

If no custom background is found, the application automatically falls back to the default `background.jpg`.
