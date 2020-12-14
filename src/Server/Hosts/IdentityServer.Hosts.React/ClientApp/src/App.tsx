import React, { Component } from 'react';
import { Route } from 'react-router';
import { Routes, Layout } from "./pages";

const App: React.FC = () => {
  return (
      <Layout>
        {Routes.map(page => (
                <Route {...page.props} key={page.title} />
         ))}
      </Layout>
  );
};

export default App;
