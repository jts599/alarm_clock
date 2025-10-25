# Alarm Clock Backend

A .NET 8 Web API backend service for an alarm clock application with LIFX smart light integration.

## Architecture

- **Backend**: .NET 8 Web API, served on port 5000
- **Containerization**: Docker with Docker Compose in host networking mode

## Development Setup

### Prerequisites
- Docker and Docker Compose (for containerized deployment)
- .NET 8 SDK (for local development)
- VS Code with C# extension (optional, for debugging)

### Running Locally (with debugging)

```bash
cd backend
dotnet run
```

The API will be available at `http://localhost:5000`

### VS Code Debugging
- Use `F5` or select ".NET Core Launch (web)" from the debug panel
- This will start the backend with full debugging support

### Running with Docker

```bash
# Build and run in production mode
docker-compose up --build

# Run in detached mode
docker-compose up -d --build

# Stop containers
docker-compose down
```

The backend runs in host networking mode, making the API directly accessible at `http://localhost:5000`

## API Documentation

When running in Development mode, Swagger UI is available at `http://localhost:5000/swagger`

## Project Structure

```
alarm_clock/
├── backend/
│   ├── src/
│   │   ├── controllers/    # API controllers
│   │   ├── models/        # Data models
│   │   └── services/      # Business logic services
│   ├── Dockerfile         # Production Dockerfile
│   ├── Dockerfile.debug   # Development Dockerfile
│   └── backend.csproj     # .NET project file
├── .vscode/
│   ├── launch.json        # VS Code debug configurations
│   └── tasks.json         # VS Code tasks
├── docker-compose.yml     # Docker Compose configuration
└── README.md             # This file
```

## Building the Application

```bash
# Build the backend
dotnet build backend/backend.csproj

# Run tests (if available)
dotnet test

# Publish for production
dotnet publish backend/backend.csproj -c Release -o ./publish
```

## Docker Commands

- `docker-compose up --build` - Build and run the backend
- `docker-compose down` - Stop and remove containers
- `docker-compose logs -f` - View logs
- `docker-compose restart` - Restart the backend service

## Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Set to `Development` or `Production`
- `ASPNETCORE_URLS` - URLs the server listens on (default: `http://0.0.0.0:5000`)

## Contributing

Contributions are welcome! Please feel free to submit a pull request or open an issue for any suggestions or improvements.

## License

This project is licensed under the MIT License.
