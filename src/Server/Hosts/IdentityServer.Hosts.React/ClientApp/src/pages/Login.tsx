import React from 'react';
import {useFormik} from "formik";
import {nameof} from "ts-simple-nameof";
import {getQueryString} from "../utils/queryString";
import {ILoginForm, ILoginQuery} from "../types";
import {login} from "../api/identity";

const formInitialValues:ILoginForm = {
    ReturnUrl: getQueryString(nameof<ILoginQuery>(p => p.ReturnUrl)),
    Username: "",
    Password: ""
}

export const Login = () => {

    const formik = useFormik({
        initialValues: formInitialValues,
        onSubmit: async values => {
            console.log('form submitted', values);

            const response = await login(values);
            console.log('response', response);
            window.location.href = response.returnUrl;
        }
        }
    );

    return (
        <form onSubmit={formik.handleSubmit}>
            <label>Username</label>
            <input
                type="text"
                name={nameof<ILoginForm>(p => p.Username)}
                onChange={formik.handleChange}
                value={formik.values.Username}
            />
            <label>Password</label>
            <input
                type="password"
                name={nameof<ILoginForm>(p => p.Password)}
                onChange={formik.handleChange}
                value={formik.values.Password}
                autoComplete="on"
            />
            <input type="submit" value="Login"/>
        </form>
    );
};
