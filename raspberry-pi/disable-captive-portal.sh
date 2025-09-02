#!/bin/bash

# Disable Captive Portal Mode
# This script switches the Pi back to normal WiFi client mode

set -e

echo "Disabling captive portal mode..."

# Stop hotspot services
sudo systemctl stop hostapd
sudo systemctl stop dnsmasq
sudo systemctl stop dhcpcd

# Stop application containers
cd /home/pi/alarm_clock 2>/dev/null || true
docker-compose -f docker-compose.captive.yml down 2>/dev/null || true

# Restore network configuration
sudo cp /etc/dhcpcd.conf.backup /etc/dhcpcd.conf 2>/dev/null || true

# Disable hotspot services
sudo systemctl disable hostapd
sudo systemctl disable dnsmasq
sudo systemctl mask hostapd

# Clear iptables rules
sudo iptables -F
sudo iptables -t nat -F
sudo iptables -X
sudo iptables -t nat -X

# Restore default iptables rules (allow all)
sudo iptables -P INPUT ACCEPT
sudo iptables -P FORWARD ACCEPT
sudo iptables -P OUTPUT ACCEPT
sudo netfilter-persistent save

# Restart normal WiFi services
sudo systemctl start dhcpcd
sudo systemctl start wpa_supplicant

# Wait for network connection
sleep 10

# Start normal application
docker-compose up -d

echo "Captive portal mode disabled!"
echo "Pi should now connect to configured WiFi network"
