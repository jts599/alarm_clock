# Alarm Clock Backend API

A .NET 8 Web API for controlling LIFX smart lights as part of an alarm clock system.

## Architecture

- **Backend**: .NET 8 Web API, served on port 5000
- **LIFX Integration**: Custom fork of LifxNet library with stability improvements
- **Shared**: API specifications and shared models
- **Containerization**: Docker with Docker Compose

## Custom Dependencies

This project uses a **custom fork of the LifxNet library** located in `LifxNet-Source/` with the following improvements:
- Debugging output commented out
- Added `ToString()` implementation for `Lifx.Color`
- Timing fixes for better stability
- Bug fixes and stabilization of the lifxlan loop

The custom fork includes commits beyond the official v2.2 release that improve reliability for continuous operation in an alarm clock system.

## Development Setup

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for local development)
- VS Code with C# and Docker extensions

### First-Time Setup

1. **Clone the repository with submodules**:
   ```bash
   git clone --recurse-submodules https://github.com/jts599/alarm_clock.git
   cd alarm_clock
   ```

   Or if you already cloned without submodules:
   ```bash
   git clone https://github.com/jts599/alarm_clock.git
   cd alarm_clock
   git submodule update --init --recursive
   ```

2. **The LifxNet-Source dependency** is automatically included as a Git submodule pointing to the custom fork with stability improvements

3. **Build the solution**:
   ```bash
   dotnet build alarm_clock.sln
   ```

### Running in Development Mode

#### Local Development (with debugging)
1. **Build and run the backend**:
   ```bash
   cd backend
   dotnet run
   ```

2. **VS Code Debugging**:
   - Use `F5` or select "Launch Backend" from the debug panel
   - This will start the backend with full debugging support

#### Docker Development Mode
```bash
# Build and run in development mode
docker-compose -f docker-compose.debug.yml up --build

# Or use the VS Code task: Ctrl+Shift+P -> "Tasks: Run Task" -> "docker-up-debug"
```

### Running in Production Mode
```bash
# Build and run production containers
docker-compose up --build

# Run in detached mode
docker-compose up -d --build
```

## Project Structure

```
alarm_clock/
├── backend/
│   ├── src/
│   │   ├── controllers/
│   │   ├── services/
│   │   └── models/
│   ├── Dockerfile (production)
│   ├── Dockerfile.debug (development)
│   └── backend.csproj
├── LifxNet-Source/          # Custom fork of LifxNet with stability improvements
│   └── src/LifxNet/
├── shared/
│   └── api-spec.json
├── .vscode/
│   ├── launch.json
│   └── tasks.json
├── docker-compose.yml (production)
├── docker-compose.debug.yml (development)
└── alarm_clock.sln
```

## Available Commands

### Docker Commands
- `docker-compose up --build` - Build and run production
- `docker-compose -f docker-compose.debug.yml up --build` - Build and run development
- `docker-compose down` - Stop and remove containers

### .NET Commands
- `dotnet build alarm_clock.sln` - Build the entire solution including LifxNet dependency
- `dotnet run --project backend/backend.csproj` - Run the backend locally

### VS Code Tasks
- **build** - Build the .NET backend
- **docker-build-debug** - Build debug Docker images
- **docker-up-debug** - Build and run debug containers

## API Endpoints

The backend provides REST API endpoints for controlling LIFX lights:

- `GET /api/status` - Health check endpoint
- `GET /api/lifx/lights` - Get all discovered LIFX lights
- `POST /api/lifx/lights/{id}/power` - Toggle light power
- `POST /api/lifx/lights/{id}/color` - Set light color

## LIFX Integration

This project uses a **custom fork of the LifxNet library** (https://github.com/jts599/LifxNet) included as a Git submodule with the following improvements over the official v2.2 release:

- **Stability improvements**: Better handling of network timeouts and connection issues
- **Enhanced debugging**: Cleaner logging output for production use  
- **Performance optimizations**: Timing fixes for more reliable light control
- **Extended functionality**: Additional color handling and string representations

The custom library is automatically built as part of the solution and doesn't require separate installation.

## Updating the LifxNet Submodule

To update to the latest version of the custom LifxNet fork:

```bash
cd LifxNet-Source
git pull origin master
cd ..
git add LifxNet-Source
git commit -m "Update LifxNet submodule"
```

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

### Note on LifxNet Dependency

If you need to modify the LifxNet library:
1. Make changes in the `LifxNet-Source/src/LifxNet/` directory
2. The changes will be automatically included when building the solution
3. Consider contributing useful changes back to the upstream LifxNet project

## License

This project is licensed under the MIT License. See the LICENSE file for more details.