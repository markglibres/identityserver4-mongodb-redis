import React from 'react';
import { Route } from 'react-router';
import { Layout } from './pages/Layout';
import { pages } from './pages';
const App = () => (
    <Layout>
      {pages.map(page => {
          return (<Route {...page.props} key={page.props.component.name}/>);
      })}
    </Layout>
);

export default App;
