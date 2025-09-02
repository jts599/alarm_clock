import React from 'react';

const MainContent: React.FC = () => {
    return (
        <main style={{
            flex: 1,
            padding: '2rem',
            textAlign: 'center'
        }}>
            <h2>Welcome to your Alarm Clock</h2>
            <p>Your alarm clock is successfully connected to WiFi!</p>

            <div style={{ marginTop: '2rem' }}>
                <h3>Current Status</h3>
                <p>✅ WiFi Connected</p>
                <p>✅ System Online</p>
                <p>⏰ Ready to set alarms</p>
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
