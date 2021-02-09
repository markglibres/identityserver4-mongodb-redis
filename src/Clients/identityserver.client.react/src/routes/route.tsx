import React from 'react';
import { Home } from "../pages/home";
import { About } from "../pages/about";
import { Switch, Route } from "react-router-dom";


const Routes = () => (
    <Switch>
        <Route exact path="/" component={Home} />
        <Route exact path="/about-us" component={About} />
    </Switch>
);

export {
    Routes
}
