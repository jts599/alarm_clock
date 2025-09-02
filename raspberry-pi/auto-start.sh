#!/bin/bash

# Auto-start script for Raspberry Pi
# This script determines whether to start in normal mode or captive portal mode

SCRIPT_DIR="/home/pi/alarm_clock/raspberry-pi"
APP_DIR="/home/pi/alarm_clock"
WIFI_INTERFACE="wlan0"
MAX_WAIT_TIME=30

echo "Starting Alarm Clock application..."

# Function to check if connected to WiFi
check_wifi_connection() {
    local connected=$(iwconfig $WIFI_INTERFACE 2>/dev/null | grep -c "ESSID:\"")
    if [ "$connected" -gt 0 ]; then
        local essid=$(iwconfig $WIFI_INTERFACE 2>/dev/null | grep "ESSID:" | cut -d'"' -f2)
        if [ "$essid" != "" ] && [ "$essid" != "off" ]; then
            return 0  # Connected
        fi
    fi
    return 1  # Not connected
}

# Function to check internet connectivity
check_internet_connection() {
    ping -c 1 -W 3 8.8.8.8 >/dev/null 2>&1
    return $?
}

# Wait a bit for network to initialize
sleep 5

# Check if already connected to WiFi
if check_wifi_connection; then
    echo "WiFi connection detected, checking internet..."
    
    # Wait for internet connection (up to MAX_WAIT_TIME seconds)
    for i in $(seq 1 $MAX_WAIT_TIME); do
        if check_internet_connection; then
            echo "Internet connection verified, starting in normal mode"
            cd $APP_DIR
            docker-compose up -d
            exit 0
        fi
        echo "Waiting for internet connection... ($i/$MAX_WAIT_TIME)"
        sleep 1
    done
    
    echo "WiFi connected but no internet, starting captive portal for reconfiguration"
else
    echo "No WiFi connection detected, starting captive portal"
fi

# Start captive portal mode
echo "Starting captive portal mode..."
cd $SCRIPT_DIR
sudo ./enable-captive-portal.sh

echo "Captive portal started. Connect to 'AlarmClock-Setup' network to configure WiFi."
