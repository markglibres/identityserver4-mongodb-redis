import React from 'react';
import { useReactOidc } from '@axa-fr/react-oidc-context';
import { NavLink } from "react-router-dom";

const TopNavigation = () => {

    const { login, logout, oidcUser } = useReactOidc();
    return (
        <>
            {!oidcUser && <>
                <button type="button" onClick={() => login()}>Login</button>
                <br />
                <NavLink to="/admin">Admin (Protected Link)</NavLink>
                <br />
            </>}
            {oidcUser && <>
                <button type="button" onClick={() => logout()}>Logout</button>
                <br />
            </>}
        </>
    );
};

export {
    TopNavigation
};
