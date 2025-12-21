# Brightness Service

Controls Raspberry Pi screen backlight brightness via sysfs interface (`/sys/class/backlight`). Provides normalized percentage (0-100) control via extension methods.

## Public API

```csharp
int GetBrightnessPercent();           // Get brightness as 0-100%
void SetBrightnessPercent(int percent); // Set brightness from 0-100%
```

Extension methods automatically convert between hardware-specific values and percentages. See [IBrightnessService.cs](./IBrightnessService.cs) and [BrightnessServiceExtensions.cs](./BrightnessServiceExtensions.cs) for full documentation.

## Implementations

- **RaspberryPiBrightnessService** - Production implementation that auto-discovers backlight device in `/sys/class/backlight`
- **MockBrightnessService** - In-memory mock for development (logs operations, no hardware access)

Toggle between implementations via `RunConfiguration.StubBrightness` in appsettings.json.

## Usage

**Controller**: `BrightnessController` provides REST API endpoints:

- `GET /api/brightness` - Current brightness percentage
- `POST /api/brightness` - Set brightness percentage
- `GET /api/brightness/max` - Hardware maximum value

**Docker**: Requires `/sys/class/backlight` volume mount (configured in all docker-compose files).

## Configuration

```json
{
  "RunConfiguration": {
    "StubBrightness": false // false = hardware, true = mock
  }
}
```

## Architecture

The service uses an abstract base class pattern:

- `FileWritingBrightnessService` - Abstract base handling file I/O logic
- `RaspberryPiBrightnessService` - Concrete class providing file paths
- `MockBrightnessService` - Alternative implementation for testing

This design allows the file reading/writing logic to be shared while concrete implementations only specify paths.
