import React from 'react';
import logo from './logo.svg';
import './App.css';
import { BrowserRouter } from 'react-router-dom';
import { Routes } from './routes';
import { AuthenticationProvider, oidcLog, InMemoryWebStorage } from '@axa-fr/react-oidc-context';
import { oidcConfig } from './configs';
import { CustomAuthCallback } from './pages/callbacks';

function App() {
  return (
      <>
          <AuthenticationProvider
            configuration={oidcConfig}
            loggerLevel={oidcLog.DEBUG}
            isEnabled={true}
            callbackComponentOverride={CustomAuthCallback}
            UserStore={InMemoryWebStorage}
          >
              <BrowserRouter>
                  <Routes />
              </BrowserRouter>
          </AuthenticationProvider>
      </>

  );
}

export default App;
