import React from 'react';

const Header: React.FC = () => {
    return (
        <header style={{
            background: '#282c34',
            color: 'white',
            padding: '1rem',
            textAlign: 'center'
        }}>
            <h1>Alarm Clock</h1>
        </header>
    );
};

export default Header;
