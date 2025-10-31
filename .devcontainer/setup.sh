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

# Return to workspace root
cd ..

# Verify installations
echo "✅ Verifying installations..."
echo "Node.js version: $(node --version)"
echo "npm version: $(npm --version)"
echo ".NET version: $(dotnet --version)"

# Set up git safe directory (if needed)
git config --global --add safe.directory $(pwd)

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
echo ""
echo "🌐 Ports:"
echo "  Backend API: http://localhost:5000"
echo "  Frontend: http://localhost:3000"