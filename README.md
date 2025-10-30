# Alarm Clock Full-Stack Application

A containerized full-stack application with React/TypeScript frontend and .NET 8 backend.

## Architecture

- **Frontend**: React with TypeScript, served on port 3000
- **Backend**: .NET 8 Web API, served on port 5000
- **Shared**: API specifications and shared models
- **Containerization**: Docker with Docker Compose

## Development Setup

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 16+ (for local development)
- VS Code with C# and Docker extensions

### Running in Development Mode

#### Option 1: Local Development (with debugging)
1. **Backend**:
   ```bash
   cd backend
   dotnet run
   ```

2. **Frontend**:
   ```bash
   cd frontend
   npm install
   npm start
   ```

3. **VS Code Debugging**:
   - Use `F5` or select "Launch Full Stack" from the debug panel
   - This will start both backend and frontend with full debugging support

#### Option 2: Docker Development Mode
```bash
# Build and run in development mode with hot reload
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

## Debug Configurations

### VS Code Debug Configurations Available:

1. **".NET Core Launch (web)"** - Debug the backend locally
2. **"Launch Chrome"** - Debug the frontend in Chrome
3. **"Launch Full Stack"** - Debug both frontend and backend simultaneously
4. **"Attach to Docker Backend"** - Attach debugger to running Docker container

### Environment Variables

#### Development (.env.development)
- `REACT_APP_API_URL=http://localhost:5000`
- `REACT_APP_ENV=development`
- `ASPNETCORE_ENVIRONMENT=Development`

#### Production
- `REACT_APP_ENV=production`
- `ASPNETCORE_ENVIRONMENT=Production`

## File Structure

```
alarm_clock/
├── frontend/
│   ├── src/
│   ├── Dockerfile (production)
│   ├── Dockerfile.debug (development)
│   └── package.json
├── backend/
│   ├── src/
│   ├── Dockerfile (production)
│   ├── Dockerfile.debug (development)
│   └── backend.csproj
├── shared/
│   └── api-spec.json
├── .vscode/
│   ├── launch.json
│   └── tasks.json
├── docker-compose.yml (production)
├── docker-compose.debug.yml (development)
└── .env.development
```

## Available Commands

### Docker Commands
- `docker-compose up --build` - Build and run production
- `docker-compose -f docker-compose.debug.yml up --build` - Build and run development
- `docker-compose down` - Stop and remove containers

### VS Code Tasks
- **build** - Build the .NET backend
- **npm: start** - Start the React development server
- **docker-build-debug** - Build debug Docker images
- **docker-up-debug** - Build and run debug containers

## API Specification

The shared API specification is located in `shared/api-spec.json` and is used by both frontend and backend for type safety and consistency.

## Project Structure

```
my-fullstack-app
├── frontend          # React/TypeScript frontend application
│   ├── src          # Source code for the frontend
│   ├── package.json  # Frontend dependencies and scripts
│   ├── tsconfig.json # TypeScript configuration
│   └── Dockerfile    # Dockerfile for building the frontend image
├── backend           # C#/.NET backend application
│   ├── src          # Source code for the backend
│   ├── backend.csproj # Backend project file
│   └── Dockerfile    # Dockerfile for building the backend image
├── shared            # Shared resources between frontend and backend
│   └── api-spec.json # API specifications
├── docker-compose.yml # Docker Compose configuration for multi-container setup
└── README.md         # Project documentation
```

## Getting Started

### Prerequisites

- Docker
- Docker Compose

### Setup

1. Clone the repository:
   ```
   git clone <repository-url>
   cd my-fullstack-app
   ```

2. Build and run the application using Docker Compose:
   ```
   docker-compose up --build
   ```

### Frontend

The frontend is built using React and TypeScript. It communicates with the backend through API calls defined in the `frontend/src/services/api.ts` file.

### Backend

The backend is built using C#/.NET and exposes various API endpoints defined in the controllers located in `backend/src/controllers/index.cs`.

### API Specifications

The API specifications are defined in the `shared/api-spec.json` file, which can be used to ensure consistency between the frontend and backend.

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.