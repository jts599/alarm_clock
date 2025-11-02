# Alarm Clock Full-Stack Application

A containerized full-stack application with React/TypeScript frontend and .NET 8 backend for controlling LIFX smart lights as part of an alarm clock system.

## Architecture

- **Frontend**: React with TypeScript and Vite, served on port 3000 (development) / 80 (production)
- **Backend**: .NET 8 Web API, served on port 5000
- **LIFX Integration**: Custom fork of LifxNet library with stability improvements
- **Shared**: API specifications and shared models
- **Containerization**: Docker with Docker Compose
- **CI/CD**: GitHub Actions with automatic Docker image publishing
- **Deployment**: Raspberry Pi with Watchtower for automatic updates

## 🚀 Quick Links

- **[Deployment Guide](DEPLOYMENT_GUIDE.md)** - Deploy to Raspberry Pi with Docker Hub
- **[CI/CD Setup](CI_CD_SETUP.md)** - GitHub Actions and branch strategy
- **[Contributing](#contributing)** - Development workflow and guidelines

## Custom Dependencies

This project uses a **custom fork of the LifxNet library** located in `LifxNet-Source/` with the following improvements:

- Debugging output commented out
- Added `ToString()` implementation for `Lifx.Color`
- Timing fixes for better stability
- Bug fixes and stabilization of the lifxlan loop

The custom fork includes commits beyond the official v2.2 release that improve reliability for continuous operation in an alarm clock system.

## Development Setup

### Prerequisites

- **For Dev Container**: Docker and VS Code with Remote-Containers extension
- **For Local Development**: Docker and Docker Compose, .NET 8 SDK, Node.js 18+
- VS Code with C# and Docker extensions

### Option 1: Dev Container Setup (Recommended)

The easiest way to get started is using the VS Code dev container which provides a fully configured development environment:

1. **Clone the repository with submodules**:

   ```bash
   git clone --recurse-submodules https://github.com/jts599/alarm_clock.git
   cd alarm_clock
   ```

2. **Open in VS Code**:

   ```bash
   code .
   ```

3. **Reopen in Container**:

   - VS Code will prompt to "Reopen in Container" when it detects the `.devcontainer` folder
   - Or manually: `Ctrl+Shift+P` → "Dev Containers: Reopen in Container"

4. **Wait for setup**: The container will automatically:

   - Install .NET 8 SDK and Node.js 20
   - Restore .NET dependencies
   - Install npm packages
   - Configure the development environment

5. **Start development**:
   - Backend: `cd backend && dotnet run` (port 5000)
   - Frontend: `cd frontend && npm run dev` (port 3000)
   - Or use VS Code debug: `F5` → "Launch Full Stack"

**Dev Container Benefits**:

- ✅ **No local setup required** - Everything runs in a container
- ✅ **Consistent environment** across different machines
- ✅ **All dependencies included** (.NET 8, Node.js 20, git, etc.)
- ✅ **VS Code extensions** pre-configured for fullstack development
- ✅ **Port forwarding** automatically configured (5000, 3000)
- ✅ **Works with stub services** - No network issues since LIFX services are mocked

### Option 2: Local Development Setup

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

## Configuration

The application uses `appsettings.json` for configuration. Key settings include:

### Weather Configuration

- **`Weather.Longitude`**: Longitude coordinate for weather/sunrise calculations
- **`Weather.Latitude`**: Latitude coordinate for weather/sunrise calculations

### Runtime Configuration

- **`RunConfiguration.StubLifx`**: Set to `true` to use mock LIFX service for development/testing
- **`RunConfiguration.FastTimescale`**: Set to `true` to accelerate time-based operations for testing

### Environment-Specific Settings

- **Development**: `appsettings.Development.json` - Uses stub services by default
- **Production**: `appsettings.json` - Uses real LIFX integration by default

Example `appsettings.Development.json`:

```json
{
  "Weather": {
    "Longitude": -74.006,
    "Latitude": 40.7128
  },
  "RunConfiguration": {
    "StubLifx": true,
    "FastTimescale": true
  }
}
```

### Running in Development Mode

#### Local Development (with debugging)

1. **Install frontend dependencies**:

   ```bash
   cd frontend
   npm install
   ```

2. **Build and run the backend**:

   ```bash
   cd backend
   dotnet run
   ```

3. **Run the frontend**:

   ```bash
   cd frontend
   npm run dev
   ```

4. **VS Code Debugging**:
   - Use `F5` or select "Launch Full Stack" from the debug panel
   - This will start both backend and frontend with full debugging support

#### Docker Development Mode

```bash
# Build and run in development mode
docker-compose -f docker-compose.debug.yml up --build

# Or use the VS Code task: Ctrl+Shift+P -> "Tasks: Run Task" -> "docker-up-debug"
```

This will start:

- Frontend on http://localhost:3000 (Vite dev server with hot reload)
- Backend on http://localhost:5000 (with API endpoints)

### Running in Production Mode

```bash
# Build and run production containers
docker-compose up --build

# Run in detached mode
docker-compose up -d --build
```

This will start:

- Frontend on http://localhost:3000 (served by nginx)
- Backend on http://localhost:5000

## Project Structure

```
alarm_clock/
├── frontend/                 # React/TypeScript frontend with Vite
│   ├── src/
│   ├── index.html
│   ├── package.json
│   ├── vite.config.ts
│   ├── Dockerfile (production)
│   ├── Dockerfile.debug (development)
│   └── nginx.conf
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

### Frontend Commands

- `npm install` - Install frontend dependencies (run from frontend/ directory)
- `npm run dev` - Start Vite development server with hot reload
- `npm run build` - Build frontend for production
- `npm run preview` - Preview production build locally

### VS Code Tasks

- **build** - Build the .NET backend
- **npm: install** - Install frontend dependencies
- **npm: dev** - Start frontend development server
- **docker-build-debug** - Build debug Docker images
- **docker-up-debug** - Build and run debug containers
- **docker-down** - Stop and remove containers

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

## Deployment

### Quick Deploy to Docker Hub

Use the provided script to build and push multi-architecture images:

```bash
# Build and push with 'latest' tag
./scripts/deploy.sh

# Build and push with custom tag
./scripts/deploy.sh v1.0.0
```

### Raspberry Pi Deployment

Full instructions in **[DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md)**:

1. **Publish Docker images** to Docker Hub
2. **Set up Raspberry Pi** with Docker and Docker Compose
3. **Configure auto-start** with systemd
4. **Enable automatic updates** with Watchtower

Once configured, your Raspberry Pi will:

- ✅ Start the application on boot
- ✅ Automatically update when you push new images
- ✅ Monitor and restart containers if they fail

### CI/CD Pipeline

Full instructions in **[CI_CD_SETUP.md](CI_CD_SETUP.md)**:

The project uses GitHub Actions for continuous integration and deployment:

- **Branch Strategy**: `main` (production) and `dev` (development)
- **Automatic Testing**: Runs on all PRs and pushes
- **Automatic Deployment**: Publishes Docker images on push to `main` or `dev`
- **Multi-Architecture**: Builds for AMD64 and ARM64 (Raspberry Pi)
- **Release Management**: Create releases with version tags (e.g., `v1.0.0`)

**Workflow:**

```
feature branch → dev branch (auto-deploy) → main branch (auto-deploy) → release tag
```

**GitHub Secrets Required:**

- `DOCKER_USERNAME` - Your Docker Hub username
- `DOCKER_PASSWORD` - Your Docker Hub access token

## Contributing

Contributions are welcome! Please follow the development workflow:

1. **Fork and clone** the repository with submodules
2. **Create a feature branch** from `dev`: `git checkout -b feature/my-feature`
3. **Make your changes** and test locally
4. **Submit a PR** targeting the `dev` branch
5. **After testing on dev**, changes will be promoted to `main`

See **[CI_CD_SETUP.md](CI_CD_SETUP.md)** for detailed workflow.

### Note on LifxNet Dependency

If you need to modify the LifxNet library:

1. Make changes in the `LifxNet-Source/src/LifxNet/` directory
2. The changes will be automatically included when building the solution
3. Consider contributing useful changes back to the upstream LifxNet project

## License

This project is licensed under the MIT License. See the LICENSE file for more details.
