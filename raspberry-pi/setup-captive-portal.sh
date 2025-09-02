#!/bin/bash

# Captive Portal Setup Script for Raspberry Pi
# This script configures the Pi to act as a WiFi hotspot with captive portal

set -e

# Configuration variables
HOTSPOT_SSID="AlarmClock-Setup"
HOTSPOT_PASSWORD="setup123"
INTERFACE="wlan0"
HOTSPOT_IP="192.168.4.1"
DHCP_RANGE_START="192.168.4.2"
DHCP_RANGE_END="192.168.4.20"
FRONTEND_PORT="3000"
BACKEND_PORT="5000"

echo "Setting up captive portal mode..."

# 1. Install required packages
sudo apt-get update
sudo apt-get install -y hostapd dnsmasq iptables-persistent

# 2. Configure hostapd (WiFi hotspot)
sudo tee /etc/hostapd/hostapd.conf > /dev/null <<EOF
interface=$INTERFACE
driver=nl80211
ssid=$HOTSPOT_SSID
hw_mode=g
channel=7
wmm_enabled=0
macaddr_acl=0
auth_algs=1
ignore_broadcast_ssid=0
wpa=2
wpa_passphrase=$HOTSPOT_PASSWORD
wpa_key_mgmt=WPA-PSK
wpa_pairwise=TKIP
rsn_pairwise=CCMP
EOF

# 3. Configure dnsmasq (DHCP and DNS)
sudo tee /etc/dnsmasq.conf > /dev/null <<EOF
interface=$INTERFACE
dhcp-range=$DHCP_RANGE_START,$DHCP_RANGE_END,255.255.255.0,24h
# Redirect all DNS queries to our captive portal
address=/#/$HOTSPOT_IP
EOF

# 4. Configure static IP for hotspot interface
sudo tee /etc/dhcpcd.conf.captive > /dev/null <<EOF
interface $INTERFACE
static ip_address=$HOTSPOT_IP/24
nohook wpa_supplicant
EOF

# 5. Set up iptables rules for captive portal
sudo tee /etc/iptables.captive.rules > /dev/null <<EOF
*nat
:PREROUTING ACCEPT [0:0]
:INPUT ACCEPT [0:0]
:OUTPUT ACCEPT [0:0]
:POSTROUTING ACCEPT [0:0]

# Redirect all HTTP traffic to our frontend
-A PREROUTING -i $INTERFACE -p tcp --dport 80 -j DNAT --to-destination $HOTSPOT_IP:$FRONTEND_PORT
-A PREROUTING -i $INTERFACE -p tcp --dport 443 -j DNAT --to-destination $HOTSPOT_IP:$FRONTEND_PORT

# Allow traffic to our services
-A PREROUTING -i $INTERFACE -p tcp --dport $FRONTEND_PORT -j ACCEPT
-A PREROUTING -i $INTERFACE -p tcp --dport $BACKEND_PORT -j ACCEPT

COMMIT

*filter
:INPUT ACCEPT [0:0]
:FORWARD ACCEPT [0:0]
:OUTPUT ACCEPT [0:0]

# Allow traffic on loopback
-A INPUT -i lo -j ACCEPT

# Allow established connections
-A INPUT -m state --state RELATED,ESTABLISHED -j ACCEPT

# Allow traffic on hotspot interface
-A INPUT -i $INTERFACE -j ACCEPT
-A FORWARD -i $INTERFACE -j ACCEPT

# Allow SSH (be careful with this in production)
-A INPUT -p tcp --dport 22 -j ACCEPT

COMMIT
EOF

echo "Captive portal configuration created!"
echo "Use 'sudo ./enable-captive-portal.sh' to activate"
echo "Use 'sudo ./disable-captive-portal.sh' to return to normal WiFi mode"
