import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Header from './components/Header';
import Footer from './components/Footer';
import MainContent from './components/MainContent';
import WiFiSetup from './components/WiFiSetup';

const App: React.FC = () => {
  const [isCaptiveMode, setIsCaptiveMode] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    // Check if we're in captive portal mode
    const mode = process.env.REACT_APP_MODE;
    if (mode === 'wifi_setup') {
      setIsCaptiveMode(true);
    }
    setIsLoading(false);
  }, []);

  const handleWiFiConfigured = () => {
    // Show success message and wait for system to switch modes
    alert('WiFi configured successfully! The device will now restart in normal mode.');
  };

  if (isLoading) {
    return (
      <div style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        fontSize: '18px'
      }}>
        Loading...
      </div>
    );
  }

  if (isCaptiveMode) {
    return <WiFiSetup onConfigured={handleWiFiConfigured} />;
  }

  return (
    <Router>
      <div>
        <Header />
        <Switch>
          <Route path="/" component={MainContent} />
          {/* Add more routes here as needed */}
        </Switch>
        <Footer />
      </div>
    </Router>
  );
};

export default App;