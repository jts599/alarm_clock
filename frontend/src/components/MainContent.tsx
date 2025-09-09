import React, { useState, useEffect } from 'react';
import { getIpAddress, getWifiStatus } from '../services/api';

interface IpInfo {
    ipAddress: string;
    interfaceName: string;
    success: boolean;
    message?: string;
}

interface WifiStatus {
    connected: boolean;
    ssid: string;
}

const MainContent: React.FC = () => {
    const [ipInfo, setIpInfo] = useState<IpInfo | null>(null);
    const [wifiStatus, setWifiStatus] = useState<WifiStatus | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchNetworkInfo = async () => {
            try {
                setLoading(true);
                setError(null);

                const [ipResponse, wifiResponse] = await Promise.all([
                    getIpAddress(),
                    getWifiStatus()
                ]);

                setIpInfo(ipResponse);
                setWifiStatus(wifiResponse);
            } catch (err) {
                console.error('Error fetching network info:', err);
                setError('Failed to load network information');
            } finally {
                setLoading(false);
            }
        };

        fetchNetworkInfo();
    }, []);

    const refreshNetworkInfo = async () => {
        setLoading(true);
        try {
            const [ipResponse, wifiResponse] = await Promise.all([
                getIpAddress(),
                getWifiStatus()
            ]);
            setIpInfo(ipResponse);
            setWifiStatus(wifiResponse);
            setError(null);
        } catch (err) {
            setError('Failed to refresh network information');
        } finally {
            setLoading(false);
        }
    };
    return (
        <main style={{
            flex: 1,
            padding: '2rem',
            textAlign: 'center'
        }}>
            <h2>Welcome to your Alarm Clock</h2>
            <p>Your alarm clock is successfully connected to WiFi!</p>

            {loading && (
                <div style={{ margin: '1rem 0', color: '#6c757d' }}>
                    Loading network information...
                </div>
            )}

            {error && (
                <div style={{
                    margin: '1rem 0',
                    padding: '0.5rem',
                    background: '#f8d7da',
                    color: '#721c24',
                    border: '1px solid #f5c6cb',
                    borderRadius: '4px'
                }}>
                    {error}
                </div>
            )}

            <div style={{ marginTop: '2rem' }}>
                <h3>Network Status</h3>

                {ipInfo && (
                    <div style={{
                        margin: '1rem 0',
                        padding: '1rem',
                        background: ipInfo.success ? '#d4edda' : '#f8d7da',
                        border: `1px solid ${ipInfo.success ? '#c3e6cb' : '#f5c6cb'}`,
                        borderRadius: '8px'
                    }}>
                        <h4>Device IP Address</h4>
                        {ipInfo.success ? (
                            <>
                                <p><strong>IP Address:</strong> {ipInfo.ipAddress}</p>
                                <p><strong>Interface:</strong> {ipInfo.interfaceName}</p>
                            </>
                        ) : (
                            <p style={{ color: '#721c24' }}>
                                {ipInfo.message || 'Unable to retrieve IP address'}
                            </p>
                        )}
                    </div>
                )}

                {wifiStatus && (
                    <div style={{
                        margin: '1rem 0',
                        padding: '1rem',
                        background: wifiStatus.connected ? '#d4edda' : '#fff3cd',
                        border: `1px solid ${wifiStatus.connected ? '#c3e6cb' : '#ffeaa7'}`,
                        borderRadius: '8px'
                    }}>
                        <h4>WiFi Connection</h4>
                        <p>
                            {wifiStatus.connected ? '✅' : '⚠️'}
                            {wifiStatus.connected ? ' Connected' : ' Disconnected'}
                        </p>
                        {wifiStatus.connected && wifiStatus.ssid && (
                            <p><strong>Network:</strong> {wifiStatus.ssid}</p>
                        )}
                    </div>
                )}

                <div style={{ marginTop: '1rem' }}>
                    <p>⏰ Ready to set alarms</p>
                </div>

                <button
                    onClick={refreshNetworkInfo}
                    disabled={loading}
                    style={{
                        margin: '0.5rem',
                        padding: '0.5rem 1rem',
                        background: loading ? '#6c757d' : '#17a2b8',
                        color: 'white',
                        border: 'none',
                        borderRadius: '4px',
                        cursor: loading ? 'not-allowed' : 'pointer'
                    }}
                >
                    {loading ? 'Refreshing...' : 'Refresh Network Info'}
                </button>
            </div>

            <div style={{
                marginTop: '2rem',
                padding: '1rem',
                background: '#f8f9fa',
                borderRadius: '8px'
            }}>
                <h4>Quick Actions</h4>
                <button style={{
                    margin: '0.5rem',
                    padding: '0.5rem 1rem',
                    background: '#007bff',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer'
                }}>
                    Set Alarm
                </button>
                <button style={{
                    margin: '0.5rem',
                    padding: '0.5rem 1rem',
                    background: '#28a745',
                    color: 'white',
                    border: 'none',
                    borderRadius: '4px',
                    cursor: 'pointer'
                }}>
                    View Settings
                </button>
            </div>
        </main>
    );
};

export default MainContent;
