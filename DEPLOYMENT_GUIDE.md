# Raspberry Pi Deployment Guide

This guide walks you through publishing your alarm clock Docker images to Docker Hub and setting up a Raspberry Pi to run them on startup with automatic updates via Watchtower.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Part 1: Build and Publish Docker Images](#part-1-build-and-publish-docker-images)
3. [Part 2: Raspberry Pi Setup](#part-2-raspberry-pi-setup)
4. [Part 3: Configure Auto-Start with Docker Compose](#part-3-configure-auto-start-with-docker-compose)
5. [Part 4: Set Up Watchtower for Automatic Updates](#part-4-set-up-watchtower-for-automatic-updates)
6. [Part 5: Verify and Monitor](#part-5-verify-and-monitor)
7. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### On Your Development Machine
- Docker installed and running
- Docker Hub account (free tier works fine)
- Git with repository cloned
- Terminal access

### For Raspberry Pi
- Raspberry Pi 4 (recommended) or Pi 3B+ with at least 2GB RAM
- Raspberry Pi OS (64-bit recommended for better .NET performance)
- Network connection (WiFi or Ethernet)
- SSH access enabled

---

## Part 1: Build and Publish Docker Images

### Step 1.1: Log in to Docker Hub

```bash
# Log in to Docker Hub from your terminal
docker login

# Enter your Docker Hub username and password when prompted
```

### Step 1.2: Choose Your Docker Hub Repository Name

Replace `your-dockerhub-username` with your actual Docker Hub username throughout this guide.

Example: If your username is `jsmith`, your images will be:
- `jsmith/alarm-clock-backend:latest`
- `jsmith/alarm-clock-frontend:latest`

### Step 1.3: Build the Backend Image

```bash
# Navigate to your project root
cd /workspaces/alarm_clock

# Build the backend image for ARM64 (Raspberry Pi)
docker buildx build --platform linux/arm64 \
  -t your-dockerhub-username/alarm-clock-backend:latest \
  -f backend/Dockerfile \
  backend/

# Optional: Also build for AMD64 if you want multi-arch support
docker buildx build --platform linux/arm64,linux/amd64 \
  -t your-dockerhub-username/alarm-clock-backend:latest \
  -f backend/Dockerfile \
  backend/
```

**Note**: If `buildx` is not available, you can build directly on the Raspberry Pi, or set it up:
```bash
# Set up buildx (one-time setup)
docker buildx create --name mybuilder --use
docker buildx inspect --bootstrap
```

### Step 1.4: Build the Frontend Image

```bash
# Build the frontend image for ARM64
docker buildx build --platform linux/arm64 \
  -t your-dockerhub-username/alarm-clock-frontend:latest \
  -f frontend/Dockerfile \
  frontend/

# Optional: Multi-arch build
docker buildx build --platform linux/arm64,linux/amd64 \
  -t your-dockerhub-username/alarm-clock-frontend:latest \
  -f frontend/Dockerfile \
  frontend/
```

### Step 1.5: Push Images to Docker Hub

```bash
# Push backend image
docker push your-dockerhub-username/alarm-clock-backend:latest

# Push frontend image
docker push your-dockerhub-username/alarm-clock-frontend:latest
```

**Verification**: Visit https://hub.docker.com/r/your-dockerhub-username/ to confirm your images are published.

### Step 1.6: Update docker-compose.yml

Update the image names in your `docker-compose.yml`:

```yaml
services:
  frontend:
    image: your-dockerhub-username/alarm-clock-frontend:latest
    # ... rest of config

  backend:
    image: your-dockerhub-username/alarm-clock-backend:latest
    # ... rest of config
```

---

## Part 2: Raspberry Pi Setup

### Step 2.1: Initial Raspberry Pi Configuration

```bash
# SSH into your Raspberry Pi
ssh pi@raspberrypi.local

# Update system packages
sudo apt update && sudo apt upgrade -y

# Install Docker if not already installed
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add your user to the docker group (avoid sudo for docker commands)
sudo usermod -aG docker $USER

# Log out and back in for group changes to take effect
exit
ssh pi@raspberrypi.local

# Verify Docker installation
docker --version
docker compose version
```

### Step 2.2: Create Project Directory

```bash
# Create directory for your alarm clock
sudo mkdir -p /opt/alarm-clock
sudo chown $USER:$USER /opt/alarm-clock
cd /opt/alarm-clock

# Create subdirectories
mkdir -p models shared logs
```

### Step 2.3: Copy Configuration Files

From your development machine, copy the necessary files:

```bash
# On your development machine (not on Pi)
# Copy docker-compose.yml to the Pi
scp docker-compose.yml pi@raspberrypi.local:/opt/alarm-clock/

# Copy shared configuration if needed
scp -r shared/* pi@raspberrypi.local:/opt/alarm-clock/shared/

# Copy any environment-specific configs
# Create a production appsettings file if needed
scp backend/appsettings.json pi@raspberrypi.local:/opt/alarm-clock/
```

### Step 2.4: Configure Environment Variables on Pi

```bash
# SSH back to the Pi
ssh pi@raspberrypi.local
cd /opt/alarm-clock

# Create a .env file for Docker Compose
cat > .env << 'EOF'
# Docker Registry
DOCKER_USERNAME=your-dockerhub-username

# Application Settings
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:5000

# Weather Configuration (update with your coordinates)
WEATHER_LONGITUDE=-74.0060
WEATHER_LATITUDE=40.7128

# LLM Settings
LLM_MODEL_PATH=/app/models/tinyllama-1.1b-chat.onnx
LLM_ENABLE=true

# LIFX Settings
STUB_LIFX=false
FAST_TIMESCALE=false
EOF

# Edit the .env file with your actual values
nano .env
```

### Step 2.5: Update docker-compose.yml Image References

```bash
# Edit docker-compose.yml to use your Docker Hub images
nano docker-compose.yml
```

Update the image references:
```yaml
services:
  frontend:
    image: ${DOCKER_USERNAME}/alarm-clock-frontend:latest
    # ...

  backend:
    image: ${DOCKER_USERNAME}/alarm-clock-backend:latest
    # ...
```

---

## Part 3: Configure Auto-Start with Docker Compose

### Step 3.1: Create Systemd Service

```bash
# Create a systemd service file
sudo nano /etc/systemd/system/alarm-clock.service
```

Add the following content:

```ini
[Unit]
Description=Alarm Clock Docker Compose Application
Requires=docker.service
After=docker.service network-online.target
Wants=network-online.target

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/alarm-clock
ExecStart=/usr/bin/docker compose up -d
ExecStop=/usr/bin/docker compose down
TimeoutStartSec=0

# Restart policy
Restart=on-failure
RestartSec=10s

[Install]
WantedBy=multi-user.target
```

### Step 3.2: Enable and Start the Service

```bash
# Reload systemd to recognize the new service
sudo systemctl daemon-reload

# Enable the service to start on boot
sudo systemctl enable alarm-clock.service

# Start the service immediately
sudo systemctl start alarm-clock.service

# Check status
sudo systemctl status alarm-clock.service

# View logs
journalctl -u alarm-clock.service -f
```

### Step 3.3: Verify Containers Are Running

```bash
# Check running containers
docker ps

# You should see:
# - alarm-clock-frontend
# - alarm-clock-backend
# - alarm-clock-model-init (may be completed)

# Check logs
docker compose logs -f
```

---

## Part 4: Set Up Watchtower for Automatic Updates

Watchtower automatically checks for updated Docker images and restarts containers with new versions.

### Step 4.1: Update docker-compose.yml to Include Watchtower

```bash
cd /opt/alarm-clock
nano docker-compose.yml
```

Add the Watchtower service to your `docker-compose.yml`:

```yaml
version: "3.8"

services:
  # ... your existing services (frontend, backend, model-init)

  watchtower:
    image: containrrr/watchtower:latest
    container_name: watchtower
    restart: unless-stopped
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
      - /etc/localtime:/etc/localtime:ro
    environment:
      # Check for updates every 5 minutes (300 seconds)
      - WATCHTOWER_POLL_INTERVAL=300
      
      # Only update containers from this compose file
      - WATCHTOWER_SCOPE=alarm-clock
      
      # Clean up old images after updating
      - WATCHTOWER_CLEANUP=true
      
      # Send notifications (optional - see notification options below)
      - WATCHTOWER_NOTIFICATIONS=shoutrrr
      - WATCHTOWER_NOTIFICATION_URL=generic://
      
      # Enable debug logging
      - WATCHTOWER_DEBUG=false
      
      # Include stopped containers
      - WATCHTOWER_INCLUDE_STOPPED=true
      
      # Monitor only containers with this label
      - WATCHTOWER_LABEL_ENABLE=true
    labels:
      - "com.centurylinklabs.watchtower.scope=alarm-clock"
    command: --interval 300 --cleanup

volumes:
  models-data:
    driver: local
    driver_opts:
      type: none
      o: bind
      device: /opt/alarm-clock/models

networks:
  default:
    driver: bridge
```

### Step 4.2: Add Labels to Your Services

Update your frontend and backend services to be monitored by Watchtower:

```yaml
  frontend:
    image: ${DOCKER_USERNAME}/alarm-clock-frontend:latest
    labels:
      - "com.centurylinklabs.watchtower.scope=alarm-clock"
      - "com.centurylinklabs.watchtower.enable=true"
    # ... rest of config

  backend:
    image: ${DOCKER_USERNAME}/alarm-clock-backend:latest
    labels:
      - "com.centurylinklabs.watchtower.scope=alarm-clock"
      - "com.centurylinklabs.watchtower.enable=true"
    # ... rest of config
```

### Step 4.3: Restart Services with Watchtower

```bash
# Restart the systemd service to apply changes
sudo systemctl restart alarm-clock.service

# Verify Watchtower is running
docker ps | grep watchtower

# Check Watchtower logs
docker logs watchtower -f
```

### Step 4.4: Configure Watchtower Notifications (Optional)

To receive notifications when Watchtower updates containers:

#### Email Notifications:
```yaml
environment:
  - WATCHTOWER_NOTIFICATIONS=email
  - WATCHTOWER_NOTIFICATION_EMAIL_FROM=alarm@yourdomain.com
  - WATCHTOWER_NOTIFICATION_EMAIL_TO=you@yourdomain.com
  - WATCHTOWER_NOTIFICATION_EMAIL_SERVER=smtp.gmail.com
  - WATCHTOWER_NOTIFICATION_EMAIL_SERVER_PORT=587
  - WATCHTOWER_NOTIFICATION_EMAIL_SERVER_USER=your-email@gmail.com
  - WATCHTOWER_NOTIFICATION_EMAIL_SERVER_PASSWORD=your-app-password
```

#### Slack Notifications:
```yaml
environment:
  - WATCHTOWER_NOTIFICATIONS=slack
  - WATCHTOWER_NOTIFICATION_SLACK_HOOK_URL=https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK
  - WATCHTOWER_NOTIFICATION_SLACK_IDENTIFIER=alarm-clock-watchtower
```

---

## Part 5: Verify and Monitor

### Step 5.1: Test Automatic Updates

On your development machine, make a small change and rebuild:

```bash
# Make a change (e.g., update a string in the UI)
# Rebuild and push
docker buildx build --platform linux/arm64 \
  -t your-dockerhub-username/alarm-clock-frontend:latest \
  -f frontend/Dockerfile \
  frontend/

docker push your-dockerhub-username/alarm-clock-frontend:latest
```

Wait up to 5 minutes (based on poll interval) and watch the Pi:

```bash
# On the Pi, watch Watchtower logs
docker logs watchtower -f

# You should see it detect, pull, and restart the frontend container
```

### Step 5.2: Useful Commands for Monitoring

```bash
# View all running containers
docker ps

# Check systemd service status
sudo systemctl status alarm-clock.service

# View container logs
docker compose logs -f

# View specific service logs
docker compose logs -f backend
docker compose logs -f frontend
docker logs watchtower

# Check resource usage
docker stats

# Restart a specific service
docker compose restart backend

# Pull latest images manually (if needed)
docker compose pull
docker compose up -d
```

### Step 5.3: Access Your Application

```bash
# Find your Pi's IP address
hostname -I

# Access the application:
# Frontend: http://<raspberry-pi-ip>:3000
# Backend API: http://<raspberry-pi-ip>:5000
```

### Step 5.4: Configure Firewall (Optional but Recommended)

```bash
# Install UFW if not already installed
sudo apt install ufw

# Allow SSH
sudo ufw allow ssh

# Allow your application ports
sudo ufw allow 3000/tcp
sudo ufw allow 5000/tcp

# Enable firewall
sudo ufw enable

# Check status
sudo ufw status
```

---

## Troubleshooting

### Containers Won't Start

```bash
# Check logs
docker compose logs

# Check specific service
docker compose logs backend

# Restart services
sudo systemctl restart alarm-clock.service
```

### Watchtower Not Updating

```bash
# Check Watchtower logs
docker logs watchtower

# Manually trigger update
docker exec watchtower /watchtower --run-once

# Verify image names match
docker compose config | grep image
```

### Out of Disk Space

```bash
# Clean up old Docker images
docker system prune -a

# Check disk usage
df -h
docker system df
```

### Systemd Service Fails to Start

```bash
# Check service status
sudo systemctl status alarm-clock.service

# View detailed logs
journalctl -u alarm-clock.service -n 50 --no-pager

# Test docker-compose manually
cd /opt/alarm-clock
docker compose up
```

### Application Not Accessible

```bash
# Check if containers are running
docker ps

# Check if ports are listening
sudo netstat -tulpn | grep -E '3000|5000'

# Check firewall
sudo ufw status

# Test from Pi itself
curl http://localhost:5000/api/status
curl http://localhost:3000
```

### Model Download Issues

```bash
# Check model-init logs
docker compose logs model-init

# Manually download model
cd /opt/alarm-clock/models
curl -L -o tinyllama-1.1b-chat.onnx \
  'https://huggingface.co/microsoft/TinyLlama-1.1B-Chat-v1.0-onnx/resolve/main/model.onnx'

# Restart services
sudo systemctl restart alarm-clock.service
```

---

## Continuous Deployment Workflow

Once everything is set up, your deployment workflow becomes:

1. **Develop** on your development machine
2. **Build** Docker images with platform flag:
   ```bash
   docker buildx build --platform linux/arm64 \
     -t your-dockerhub-username/alarm-clock-backend:latest \
     -f backend/Dockerfile backend/ --push
   ```
3. **Push** to Docker Hub (or use `--push` flag in build)
4. **Wait** for Watchtower to automatically update the Pi (5 minutes max)
5. **Verify** the update via logs or by accessing the application

### Quick Build & Push Script

Create a script to automate building and pushing:

```bash
# On your development machine
cat > deploy.sh << 'EOF'
#!/bin/bash
set -e

DOCKER_USERNAME="your-dockerhub-username"

echo "Building and pushing backend..."
docker buildx build --platform linux/arm64,linux/amd64 \
  -t $DOCKER_USERNAME/alarm-clock-backend:latest \
  -f backend/Dockerfile backend/ --push

echo "Building and pushing frontend..."
docker buildx build --platform linux/arm64,linux/amd64 \
  -t $DOCKER_USERNAME/alarm-clock-frontend:latest \
  -f frontend/Dockerfile frontend/ --push

echo "Done! Watchtower will update your Pi within 5 minutes."
EOF

chmod +x deploy.sh
```

Then simply run:
```bash
./deploy.sh
```

---

## Advanced Configuration

### Using Docker Secrets for Sensitive Data

If you need to store API keys or passwords:

```bash
# On the Pi
cd /opt/alarm-clock
echo "your-api-key" | docker secret create api_key -
```

Update docker-compose.yml:
```yaml
secrets:
  api_key:
    external: true

services:
  backend:
    secrets:
      - api_key
```

### Setting Up HTTPS with Let's Encrypt

For production access over HTTPS, consider adding a reverse proxy like Traefik or nginx with Let's Encrypt.

### Monitoring with Portainer (Optional)

For a web UI to manage Docker containers:

```bash
docker run -d -p 9000:9000 --name portainer \
  --restart=always \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data \
  portainer/portainer-ce:latest
```

Access at: http://<raspberry-pi-ip>:9000

---

## Summary

You now have:
- ✅ Docker images published to Docker Hub
- ✅ Raspberry Pi configured to run your application
- ✅ Auto-start on boot via systemd
- ✅ Automatic updates via Watchtower
- ✅ Monitoring and logging capabilities

Your alarm clock will now start automatically when the Pi boots and will automatically update whenever you push new images to Docker Hub!
