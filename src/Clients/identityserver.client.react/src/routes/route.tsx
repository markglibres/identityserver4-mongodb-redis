import React from 'react';
import { Home } from "../pages/home";
import { About } from "../pages/about";
import { Switch, Route } from "react-router-dom";
import { withOidcSecure, OidcSecure } from '@axa-fr/react-oidc-context';
import { AdminDashboard } from '../pages/admin';

const Routes = () => (
    <Switch>
        <Route exact path="/" component={Home} />
        <Route exact path="/about-us" component={About} />
        <Route exact path="/admin">
            <OidcSecure>
                <AdminDashboard />
            </OidcSecure>
        </Route>
    </Switch>
);

export {
    Routes
}
