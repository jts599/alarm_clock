#!/bin/bash

# Raspberry Pi setup script for alarm clock with Watchtower
# Run this once on your Pi to set up the environment

set -e

echo "🍓 Setting up Alarm Clock on Raspberry Pi..."

# Create persistent directories
sudo mkdir -p /opt/alarm-clock/models
sudo mkdir -p /opt/alarm-clock/data
sudo chown -R 1000:1000 /opt/alarm-clock

# Create docker-compose.yml in persistent location
sudo tee /opt/alarm-clock/docker-compose.yml > /dev/null << 'EOF'
version: '3.8'

services:
  # Watchtower for automatic updates
  watchtower:
    image: containrrr/watchtower
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
    environment:
      - WATCHTOWER_CLEANUP=true
      - WATCHTOWER_POLL_INTERVAL=300  # Check every 5 minutes
      - WATCHTOWER_INCLUDE_RESTARTING=true
    restart: unless-stopped

  # Model downloader - runs once to ensure models exist
  model-init:
    image: curlimages/curl:latest
    volumes:
      - models-data:/models
    command: >
      sh -c "
        if [ ! -f /models/tinyllama-1.1b-chat.onnx ]; then
          echo 'Downloading TinyLlama model...';
          curl -L -o /models/tinyllama-1.1b-chat.onnx 
            'https://huggingface.co/microsoft/TinyLlama-1.1B-Chat-v1.0-onnx/resolve/main/model.onnx' ||
          echo 'Fallback: Model will be downloaded on first run';
        else
          echo 'Model already exists';
        fi
      "
    restart: "no"

  frontend:
    image: your-registry/alarm-clock-frontend:latest
    ports:
      - "3000:80"
    depends_on:
      model-init:
        condition: service_completed_successfully
      backend:
        condition: service_started
    environment:
      - NODE_ENV=production
    restart: unless-stopped
    labels:
      - "com.centurylinklabs.watchtower.enable=true"

  backend:
    image: your-registry/alarm-clock-backend:latest
    ports:
      - "5000:5000"
    volumes:
      - models-data:/app/models
      - /opt/alarm-clock/data:/app/data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://0.0.0.0:5000
      - Llm__ModelPath=/app/models/tinyllama-1.1b-chat.onnx
      - Llm__EnableLlm=true
    depends_on:
      model-init:
        condition: service_completed_successfully
    restart: unless-stopped
    labels:
      - "com.centurylinklabs.watchtower.enable=true"

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
EOF

# Set up systemd service for auto-start
sudo tee /etc/systemd/system/alarm-clock.service > /dev/null << 'EOF'
[Unit]
Description=Alarm Clock Docker Compose
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/opt/alarm-clock
ExecStart=/usr/bin/docker-compose up -d
ExecStop=/usr/bin/docker-compose down
TimeoutStartSec=0

[Install]
WantedBy=multi-user.target
EOF

# Enable and start service
sudo systemctl daemon-reload
sudo systemctl enable alarm-clock.service

echo "✅ Setup complete!"
echo ""
echo "To start the alarm clock:"
echo "  sudo systemctl start alarm-clock"
echo ""
echo "To check status:"
echo "  sudo systemctl status alarm-clock"
echo "  docker ps"
echo ""
echo "Watchtower will automatically update containers when you push new images."
echo "Models are persisted in /opt/alarm-clock/models"