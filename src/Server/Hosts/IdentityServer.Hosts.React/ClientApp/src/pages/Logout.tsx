import React, {useEffect, useState} from 'react';
import {getQueryString} from "../utils/queryString";
import {nameof} from "ts-simple-nameof";
import {ILogoutForm, ILogoutQuery} from "../types";
import {login, logout} from "../api/identity";

const initialModelValues = {
    postLogoutRedirectUri: '',
    clientName: ''
}
export const Logout = () => {

    const [ model, setModel ] = useState(initialModelValues);

    useEffect(() => {
        const logoutUser = async (logoutForm: ILogoutForm) => {
            const response = await logout(logoutForm);
            setModel(response);
            console.log('logout response', response);
        };
        const logoutId = getQueryString(nameof<ILogoutQuery>(p => p.logoutId));
        logoutUser({ LogoutId: logoutId })

    }, []);

    return (
        <>
            {!model.postLogoutRedirectUri && <p>Logging out... </p>}

            {model.postLogoutRedirectUri &&
            <h1>
                Logout
                <small>You are now logged out</small>
            </h1>
            }

            {model.postLogoutRedirectUri &&

            <div>
                Click <a href={model.postLogoutRedirectUri}>here</a> to return to the
                <span>{model.clientName}</span> application.
            </div>
            }
        </>
    );
};
