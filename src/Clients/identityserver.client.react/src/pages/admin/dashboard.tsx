import React from 'react';
import { useReactOidc } from '@axa-fr/react-oidc-context';

const AdminDashboard = () => {
    const { oidcUser } = useReactOidc();
    return (
        <>
            <p>Admin dashboard.. needs valid login</p>
            <p>User:</p>
            {console.log('this is it')}
            {Object.entries(oidcUser).map(([key, value]) => (<p>{key}: {JSON.stringify(value)}</p>))}
        </>
    );
};

export {
    AdminDashboard
};
