import React, { useState, useEffect } from 'react';
import './WiFiSetup.css';

interface WiFiNetwork {
    ssid: string;
    signal: number;
    security: string;
    frequency: number;
}

interface WiFiSetupProps {
    onConfigured: () => void;
}

const WiFiSetup: React.FC<WiFiSetupProps> = ({ onConfigured }) => {
    const [networks, setNetworks] = useState<WiFiNetwork[]>([]);
    const [selectedNetwork, setSelectedNetwork] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [isScanning, setIsScanning] = useState<boolean>(false);
    const [isConnecting, setIsConnecting] = useState<boolean>(false);
    const [error, setError] = useState<string>('');
    const [connectionStatus, setConnectionStatus] = useState<string>('');

    useEffect(() => {
        scanNetworks();
    }, []);

    const scanNetworks = async () => {
        setIsScanning(true);
        setError('');
        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/wifi/scan`);
            if (!response.ok) {
                throw new Error('Failed to scan networks');
            }
            const data = await response.json();
            setNetworks(data.networks || []);
        } catch (err) {
            setError('Failed to scan for WiFi networks');
            console.error('WiFi scan error:', err);
        } finally {
            setIsScanning(false);
        }
    };

    const connectToNetwork = async () => {
        if (!selectedNetwork || !password) {
            setError('Please select a network and enter password');
            return;
        }

        setIsConnecting(true);
        setError('');
        setConnectionStatus('Connecting...');

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/wifi/connect`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    ssid: selectedNetwork,
                    password: password,
                }),
            });

            if (!response.ok) {
                throw new Error('Failed to connect to network');
            }

            const result = await response.json();

            if (result.success) {
                setConnectionStatus('Connected! Switching to normal mode...');
                setTimeout(() => {
                    onConfigured();
                }, 3000);
            } else {
                setError(result.message || 'Failed to connect to network');
            }
        } catch (err) {
            setError('Failed to connect to network');
            console.error('WiFi connection error:', err);
        } finally {
            setIsConnecting(false);
        }
    };

    const getSignalStrength = (signal: number): string => {
        if (signal > -50) return 'excellent';
        if (signal > -60) return 'good';
        if (signal > -70) return 'fair';
        return 'poor';
    };

    return (
        <div className="wifi-setup">
            <div className="wifi-setup-container">
                <h1>WiFi Setup</h1>
                <p>Connect your Alarm Clock to a WiFi network</p>

                {error && <div className="error-message">{error}</div>}
                {connectionStatus && <div className="status-message">{connectionStatus}</div>}

                <div className="scan-section">
                    <button
                        onClick={scanNetworks}
                        disabled={isScanning}
                        className="scan-button"
                    >
                        {isScanning ? 'Scanning...' : 'Scan for Networks'}
                    </button>
                </div>

                <div className="networks-section">
                    <h3>Available Networks</h3>
                    {networks.length === 0 && !isScanning ? (
                        <p>No networks found. Try scanning again.</p>
                    ) : (
                        <div className="networks-list">
                            {networks.map((network, index) => (
                                <div
                                    key={index}
                                    className={`network-item ${selectedNetwork === network.ssid ? 'selected' : ''}`}
                                    onClick={() => setSelectedNetwork(network.ssid)}
                                >
                                    <div className="network-info">
                                        <div className="network-ssid">{network.ssid}</div>
                                        <div className="network-details">
                                            <span className={`signal ${getSignalStrength(network.signal)}`}>
                                                Signal: {network.signal}dBm
                                            </span>
                                            <span className="security">{network.security}</span>
                                        </div>
                                    </div>
                                    <div className="signal-bars">
                                        {[...Array(4)].map((_, i) => (
                                            <div
                                                key={i}
                                                className={`bar ${network.signal > -70 + (i * 10) ? 'active' : ''}`}
                                            />
                                        ))}
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {selectedNetwork && (
                    <div className="connection-section">
                        <h3>Connect to {selectedNetwork}</h3>
                        <div className="password-input">
                            <label htmlFor="password">Password:</label>
                            <input
                                type="password"
                                id="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="Enter WiFi password"
                                disabled={isConnecting}
                            />
                        </div>
                        <button
                            onClick={connectToNetwork}
                            disabled={isConnecting || !password}
                            className="connect-button"
                        >
                            {isConnecting ? 'Connecting...' : 'Connect'}
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
};

export default WiFiSetup;
