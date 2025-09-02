#!/bin/bash

# Enable Captive Portal Mode
# This script switches the Pi from normal WiFi client mode to hotspot mode

set -e

echo "Enabling captive portal mode..."

# Stop normal WiFi services
sudo systemctl stop wpa_supplicant
sudo systemctl stop dhcpcd

# Backup current network configuration
sudo cp /etc/dhcpcd.conf /etc/dhcpcd.conf.backup 2>/dev/null || true
sudo cp /etc/wpa_supplicant/wpa_supplicant.conf /etc/wpa_supplicant/wpa_supplicant.conf.backup 2>/dev/null || true

# Apply captive portal network configuration
sudo cp /etc/dhcpcd.conf.captive /etc/dhcpcd.conf

# Apply iptables rules for captive portal
sudo iptables-restore < /etc/iptables.captive.rules
sudo netfilter-persistent save

# Start hotspot services
sudo systemctl unmask hostapd
sudo systemctl enable hostapd
sudo systemctl enable dnsmasq

# Restart networking
sudo systemctl start dhcpcd
sudo systemctl start hostapd
sudo systemctl start dnsmasq

# Start the application containers
cd /home/pi/alarm_clock
docker-compose -f docker-compose.captive.yml up -d

echo "Captive portal mode enabled!"
echo "Connect to WiFi network: AlarmClock-Setup"
echo "Password: setup123"
echo "Navigate to any website to access the setup interface"
