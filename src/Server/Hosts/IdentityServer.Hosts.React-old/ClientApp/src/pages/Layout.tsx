import React from 'react';

export const Layout: React.FC = ({ children, ...props}) => {
  return (
      <div {...props}>
        {children}
      </div>
  );
};
