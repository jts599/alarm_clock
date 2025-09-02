# Raspberry Pi Captive Portal Setup Guide

This guide walks you through setting up the alarm clock application with captive portal functionality on a Raspberry Pi.

## Prerequisites

- Raspberry Pi 4 (or 3B+) with Raspberry Pi OS
- SD card (16GB minimum)
- WiFi capability
- Docker and Docker Compose installed

## Installation Steps

### 1. Clone the Repository
```bash
cd /home/pi
git clone <your-repo-url> alarm_clock
cd alarm_clock
```

### 2. Install Required System Packages
```bash
sudo apt update
sudo apt install -y hostapd dnsmasq iptables-persistent docker.io docker-compose
sudo usermod -aG docker pi
```

### 3. Make Scripts Executable
```bash
cd raspberry-pi
chmod +x *.sh
```

### 4. Run Initial Setup
```bash
sudo ./setup-captive-portal.sh
```

### 5. Install Systemd Service
```bash
# Copy service file
sudo cp alarm-clock.service /etc/systemd/system/

# Enable auto-start
sudo systemctl enable alarm-clock.service

# Start the service
sudo systemctl start alarm-clock.service
```

### 6. Configure Auto-Start (Optional)
To make the captive portal start automatically on boot:

```bash
# Add to /etc/rc.local (before 'exit 0')
echo '/home/pi/alarm_clock/raspberry-pi/auto-start.sh &' | sudo tee -a /etc/rc.local
```

## How It Works

### Normal Operation Mode
1. Pi connects to configured WiFi network
2. Verifies internet connectivity
3. Starts the full application with frontend and backend
4. Application is accessible via the Pi's IP address

### Captive Portal Mode
1. Triggered when:
   - No WiFi configuration exists
   - WiFi connection fails
   - No internet connectivity detected
2. Pi creates WiFi hotspot: `AlarmClock-Setup` (password: `setup123`)
3. All HTTP requests redirect to the WiFi setup interface
4. User selects WiFi network and enters credentials
5. Pi switches back to normal mode automatically

### Mode Detection Logic
```bash
# The auto-start script checks:
1. WiFi connection status
2. Internet connectivity (ping test)
3. Decides between normal mode or captive portal
```

## Configuration Files

- **`/etc/hostapd/hostapd.conf`** - WiFi hotspot configuration
- **`/etc/dnsmasq.conf`** - DHCP and DNS for captive portal
- **`/etc/iptables.captive.rules`** - Network routing rules
- **`docker-compose.captive.yml`** - Container configuration for captive mode

## Usage

### Connecting to WiFi (First Time Setup)
1. Power on the Raspberry Pi
2. Wait 1-2 minutes for boot and initialization
3. Connect your device to WiFi network `AlarmClock-Setup`
4. Password: `setup123`
5. Open any website in your browser
6. You'll be redirected to the WiFi setup page
7. Select your WiFi network and enter the password
8. Click "Connect"
9. Pi will restart in normal mode

### Reconfiguring WiFi
If you need to change WiFi settings:
```bash
# Manually trigger captive portal mode
sudo /home/pi/alarm_clock/raspberry-pi/enable-captive-portal.sh
```

### Switching Back to Normal Mode
```bash
# Manually return to normal mode
sudo /home/pi/alarm_clock/raspberry-pi/disable-captive-portal.sh
```

## Monitoring and Debugging

### Check Service Status
```bash
sudo systemctl status alarm-clock
```

### View Logs
```bash
# System logs
sudo journalctl -u alarm-clock -f

# Application logs
docker-compose logs -f
```

### Check Network Status
```bash
# WiFi status
iwconfig wlan0

# Hotspot status
sudo systemctl status hostapd
sudo systemctl status dnsmasq

# IP tables rules
sudo iptables -L -n -t nat
```

### Manual Network Scan
```bash
# Scan for networks
sudo iwlist wlan0 scan | grep ESSID
```

## Security Considerations

1. **Change Default Hotspot Password**: Edit `HOTSPOT_PASSWORD` in `setup-captive-portal.sh`
2. **SSH Access**: Ensure SSH is properly configured for remote access
3. **Firewall Rules**: The captive portal temporarily modifies iptables rules
4. **WiFi Credentials**: Stored in `/etc/wpa_supplicant/wpa_supplicant.conf`

## Troubleshooting

### Pi Stuck in Captive Portal Mode
```bash
# Check WiFi configuration
sudo cat /etc/wpa_supplicant/wpa_supplicant.conf

# Test WiFi connection manually
sudo wpa_cli reconfigure
```

### Cannot Access Setup Interface
```bash
# Check if services are running
sudo systemctl status hostapd dnsmasq

# Check IP configuration
ip addr show wlan0

# Check iptables rules
sudo iptables -L -n -t nat
```

### Docker Issues
```bash
# Restart Docker
sudo systemctl restart docker

# Check container status
docker-compose ps

# View container logs
docker-compose logs backend
docker-compose logs frontend
```

## Network Architecture

```
Internet ←→ Router ←→ [Normal Mode] Raspberry Pi ←→ Application
                                    ↓
                     [Captive Portal Mode]
                           WiFi Hotspot
                                    ↓
                              User Device ←→ Setup Interface
```

This setup provides a seamless experience where the device automatically determines the appropriate mode based on network connectivity status.
