#!/bin/bash
set -e

echo "🚀 Generating TypeScript API client..."

# Navigate to backend directory
cd "$(dirname "$0")/../backend"

echo "📦 Starting backend server..."
# Start the backend in the background
dotnet run --no-build &
SERVER_PID=$!

# Function to cleanup on exit
cleanup() {
    echo "🛑 Stopping server..."
    kill $SERVER_PID 2>/dev/null || true
}
trap cleanup EXIT

# Wait for server to be ready
echo "⏳ Waiting for server to start..."
for i in {1..30}; do
    if curl -s http://localhost:5000/swagger/v1/swagger.json > /dev/null 2>&1; then
        echo "✅ Server is ready!"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "❌ Server failed to start within 30 seconds"
        exit 1
    fi
    sleep 1
done

echo "📥 Downloading OpenAPI specification..."
curl -s http://localhost:5000/swagger/v1/swagger.json > /tmp/api-spec.json

echo "🔧 Generating TypeScript client..."
cd ../shared

# Create api directory if it doesn't exist
mkdir -p api

# Remove stale generated files to avoid leftover/removed endpoints lingering in the client
GENERATED_DIR="$(pwd)/api/generated"
if [ -d "$GENERATED_DIR" ]; then
    echo "🧹 Removing stale generated client at $GENERATED_DIR"
    rm -rf "$GENERATED_DIR"
fi
mkdir -p "$GENERATED_DIR"

# Generate TypeScript client using npx (no global install needed)
npx @openapitools/openapi-generator-cli generate \
  -i /tmp/api-spec.json \
  -g typescript-fetch \
  -o api/generated \
  --additional-properties=typescriptThreePlus=true,supportsES6=true

# Create a simple re-export file
cat > api/index.ts << 'EOF'
export * from './generated';
export { AlarmSettingsApi as AlarmClockApiClient } from './generated';
EOF

echo "✅ TypeScript client generated successfully!"
echo "📁 Client files are in shared/api/"

# Cleanup temp file
rm -f /tmp/api-spec.json

echo "🎉 Done!"