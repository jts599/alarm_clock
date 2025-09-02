import React, { useState, useEffect } from 'react';

interface NetworkInfo {
  ipAddress: string;
  interfaceName: string;
  success: boolean;
}

const App: React.FC = () => {
  const [networkInfo, setNetworkInfo] = useState<NetworkInfo | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchNetworkInfo = async () => {
      try {
        const apiUrl = process.env.REACT_APP_API_URL || '/api';
        const endpoint = `${apiUrl}/wifi/ip_address`;
        console.log('Fetching network info from:', endpoint);
        
        const response = await fetch(endpoint);
        console.log('Response status:', response.status);
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const data = await response.json();
        console.log('Network info received:', data);
        setNetworkInfo(data);
      } catch (error) {
        console.error('Failed to fetch network info:', error);
        setNetworkInfo({ 
          ipAddress: `Error: ${error instanceof Error ? error.message : 'Unknown error'}`, 
          interfaceName: 'unknown', 
          success: false 
        });
      } finally {
        setLoading(false);
      }
    };

    fetchNetworkInfo();
  }, []);

  if (loading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '100vh',
        fontFamily: 'Arial, sans-serif'
      }}>
        <div>Loading network information...</div>
      </div>
    );
  }

  return (
    <div style={{ 
      padding: '20px', 
      fontFamily: 'Arial, sans-serif',
      maxWidth: '800px',
      margin: '0 auto'
    }}>
      <header style={{
        backgroundColor: '#282c34',
        color: 'white',
        padding: '20px',
        borderRadius: '8px',
        marginBottom: '20px',
        textAlign: 'center'
      }}>
        <h1>🚨 Alarm Clock Setup 🚨</h1>
      </header>

      <div style={{
        backgroundColor: '#f8f9fa',
        padding: '20px',
        borderRadius: '8px',
        border: '1px solid #dee2e6'
      }}>
        <h2>Network Information</h2>
        {networkInfo && (
          <div>
            <p><strong>IP Address:</strong> {networkInfo.ipAddress}</p>
            <p><strong>Interface:</strong> {networkInfo.interfaceName}</p>
            <p><strong>Status:</strong> {networkInfo.success ? '✅ Connected' : '❌ Error'}</p>
          </div>
        )}
      </div>

      <div style={{ marginTop: '20px', textAlign: 'center' }}>
        <p>Connect to WiFi network: <strong>{networkInfo?.ipAddress || 'Loading...'}</strong></p>
        <button 
          onClick={() => window.location.reload()} 
          style={{ 
            padding: '10px 20px', 
            margin: '10px',
            backgroundColor: '#007bff',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer'
          }}
        >
          Refresh Network Info
        </button>
        <button 
          onClick={async () => {
            try {
              const apiUrl = process.env.REACT_APP_API_URL || '/api';
              const endpoint = `${apiUrl}/wifi/ip_address`;
              const response = await fetch(endpoint);
              const data = await response.json();
              alert(`Manual test result: ${JSON.stringify(data, null, 2)}`);
            } catch (error) {
              alert(`Manual test failed: ${error}`);
            }
          }} 
          style={{ 
            padding: '10px 20px', 
            margin: '10px',
            backgroundColor: '#28a745',
            color: 'white',
            border: 'none',
            borderRadius: '4px',
            cursor: 'pointer'
          }}
        >
          Test API Connection
        </button>
      </div>
    </div>
  );
};

export default App;