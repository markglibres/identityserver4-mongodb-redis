import React from 'react';
import logo from './logo.svg';
import './App.css';
import { BrowserRouter } from 'react-router-dom';
import { Routes } from './routes';
import { AuthenticationProvider, oidcLog, InMemoryWebStorage } from '@axa-fr/react-oidc-context';
import { oidcConfig } from './configs';
import { CustomAuthCallback, NotAuthenticated } from './pages/callbacks';
import { TopNavigation } from './layout/navigation';

function App() {
  return (
      <>
          <AuthenticationProvider
              notAuthenticated={NotAuthenticated}
            configuration={oidcConfig}
            loggerLevel={oidcLog.DEBUG}
            isEnabled={true}
            callbackComponentOverride={CustomAuthCallback}
            UserStore={InMemoryWebStorage}
          >
              <BrowserRouter>
                  <TopNavigation />
                  <Routes />
              </BrowserRouter>
          </AuthenticationProvider>
      </>

  );
}

export default App;
