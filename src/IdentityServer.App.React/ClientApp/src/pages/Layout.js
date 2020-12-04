import React from 'react';
import { Container } from 'reactstrap';

export const Layout = ({ children, ...props}) => {
  return (
      <Container {...props}>
        {children}
      </Container>
  );
};
