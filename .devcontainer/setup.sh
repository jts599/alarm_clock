#!/bin/bash

echo "🚀 Setting up Alarm Clock Fullstack Development Environment..."

# We're already in the workspace directory, no need to change directories

# Setup Backend
echo "📦 Restoring .NET dependencies..."
dotnet restore backend/backend.csproj

# Setup Frontend
echo "📦 Installing npm dependencies..."
cd frontend
npm install

# Install OpenAPI Generator CLI globally
echo "🔧 Installing OpenAPI Generator CLI..."
npm install -g @openapitools/openapi-generator-cli@2.25.0

# Return to workspace root
cd ..

# Verify installations
echo "✅ Verifying installations..."
echo "Node.js version: $(node --version)"
echo "npm version: $(npm --version)"
echo ".NET version: $(dotnet --version)"
echo "Java version: $(java -version 2>&1 | head -n 1)"
echo "OpenAPI Generator CLI: $(openapi-generator-cli version 2>/dev/null || echo 'Installing on first use...')"

# Set up git safe directory (if needed)
git config --global --add safe.directory $(pwd)

# Configure VS Code as the default merge tool
echo "⚙️  Configuring VS Code as git merge tool..."
git config --global merge.tool vscode
git config --global mergetool.vscode.cmd 'code --wait $MERGED'
git config --global mergetool.vscode.trustExitCode true
git config --global mergetool.keepBackup false

# Initialize and update git submodules
echo "🔗 Initializing git submodules..."
git submodule init
git submodule update

echo "🎉 Development environment setup complete!"
echo ""
echo "📋 Available commands:"
echo "  Backend:"
echo "    cd backend && dotnet run"
echo "    cd backend && dotnet build"
echo "  Frontend:"
echo "    cd frontend && npm run dev"
echo "    cd frontend && npm run build"
echo "  API Client Generation:"
echo "    ./scripts/generate-api-client.sh"
echo ""
echo "🌐 Ports:"
echo "  Backend API: http://localhost:5000"
echo "  Frontend: http://localhost:3000"