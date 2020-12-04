import React, { Component } from 'react';
import { Route } from 'react-router';
import { Routes, Layout } from "./pages";

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        {Routes.map(page => (
                <Route {...page.props} key={page.title} />
         ))}
      </Layout>
    );
  }
}
