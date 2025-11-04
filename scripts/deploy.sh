#!/bin/bash
set -e

# Quick Deploy Script for Alarm Clock
# Builds and pushes Docker images to Docker Hub

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
DOCKER_USERNAME="${DOCKER_USERNAME:-}"
BUILD_PLATFORMS="linux/amd64,linux/arm64"

# Functions
print_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

check_requirements() {
    print_info "Checking requirements..."
    
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed"
        exit 1
    fi
    
    if ! docker buildx version &> /dev/null; then
        print_error "Docker buildx is not available"
        print_info "Setting up buildx..."
        docker buildx create --name mybuilder --use 2>/dev/null || true
        docker buildx inspect --bootstrap
    fi
    
    print_info "✓ All requirements met"
}

get_docker_username() {
    if [ -z "$DOCKER_USERNAME" ]; then
        echo ""
        read -p "Enter your Docker Hub username: " DOCKER_USERNAME
        if [ -z "$DOCKER_USERNAME" ]; then
            print_error "Docker Hub username is required"
            exit 1
        fi
    fi
}

docker_login() {
    print_info "Checking Docker Hub authentication..."
    
    if ! docker info 2>/dev/null | grep -q "Username: $DOCKER_USERNAME"; then
        print_warning "Not logged in to Docker Hub"
        echo ""
        docker login
    else
        print_info "✓ Already logged in as $DOCKER_USERNAME"
    fi
}

build_and_push() {
    local component=$1
    local context=$2
    local dockerfile=$3
    local tag=$4
    
    print_info "Building and pushing $component..."
    print_info "  Image: $DOCKER_USERNAME/alarm-clock-$component:$tag"
    print_info "  Platforms: $BUILD_PLATFORMS"
    
    docker buildx build \
        --platform "$BUILD_PLATFORMS" \
        -t "$DOCKER_USERNAME/alarm-clock-$component:$tag" \
        -f "$dockerfile" \
        "$context" \
        --push
    
    print_info "✓ $component pushed successfully"
}

update_compose_files() {
    print_info "Updating docker-compose files with your username..."
    
    # Update docker-compose.yml
    if [ -f "docker-compose.yml" ]; then
        sed -i.bak "s/your-dockerhub-username/$DOCKER_USERNAME/g" docker-compose.yml
        print_info "✓ Updated docker-compose.yml"
    fi
    
    # Update docker-compose.prod.yml
    if [ -f "docker-compose.prod.yml" ]; then
        sed -i.bak "s/your-dockerhub-username/$DOCKER_USERNAME/g" docker-compose.prod.yml
        print_info "✓ Updated docker-compose.prod.yml"
    fi
    
    # Update docker-compose.dev.yml
    if [ -f "docker-compose.dev.yml" ]; then
        sed -i.bak "s/your-dockerhub-username/$DOCKER_USERNAME/g" docker-compose.dev.yml
        print_info "✓ Updated docker-compose.dev.yml"
    fi
}

show_summary() {
    echo ""
    echo "================================================"
    echo -e "${GREEN}Deployment Complete!${NC}"
    echo "================================================"
    echo ""
    echo "Images published:"
    echo "  - $DOCKER_USERNAME/alarm-clock-backend:$TAG"
    echo "  - $DOCKER_USERNAME/alarm-clock-frontend:$TAG"
    echo ""
    echo "Platforms:"
    echo "  - linux/amd64 (x86_64)"
    echo "  - linux/arm64 (Raspberry Pi)"
    echo ""
    echo "Next steps:"
    echo "  1. Set up your Raspberry Pi (see DEPLOYMENT_GUIDE.md)"
    echo "  2. Configure GitHub Secrets for CI/CD (see CI_CD_SETUP.md)"
    echo "  3. Watchtower will auto-update within 5 minutes"
    echo ""
}

# Main script
main() {
    echo "================================================"
    echo "  Alarm Clock - Docker Build & Push Script"
    echo "================================================"
    echo ""
    
    # Get tag from argument or use "latest"
    TAG="${1:-latest}"
    
    print_info "Building for tag: $TAG"
    echo ""
    
    # Run checks
    check_requirements
    get_docker_username
    docker_login
    
    echo ""
    print_info "Starting build process..."
    echo ""
    
    # Build and push backend
    build_and_push "backend" "./backend" "./backend/Dockerfile" "$TAG"
    echo ""
    
    # Build and push frontend
    build_and_push "frontend" "./frontend" "./frontend/Dockerfile" "$TAG"
    echo ""
    
    # Update compose files if first time
    if grep -q "your-dockerhub-username" docker-compose.yml 2>/dev/null; then
        echo ""
        update_compose_files
    fi
    
    # Show summary
    show_summary
}

# Run main function
main "$@"
