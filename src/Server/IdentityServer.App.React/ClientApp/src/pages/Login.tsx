import React from 'react';
import {useFormik} from "formik";
import {Error} from "../design-system/atoms/Error";
import * as Yup from 'yup';
import {emailAddressSchema, passwordSchema} from "../validations/schema";
import {AccountsApi} from "../api/identityManagement";
import queryString from 'query-string';
import {useLocation} from "react-router";

interface FormLoginProps {
    email: string;
    password: string
}
const formLoginInitialValues = {
    email: '',
    password: ''
}

export const Login: React.FC = ({...loginProps}) => {

    const location = useLocation();
    const queryStrings = queryString.parse(location.search);

    const { errors, handleSubmit, handleChange } = useFormik<FormLoginProps>({
        initialValues: formLoginInitialValues,
        validationSchema: Yup.object({
            email: emailAddressSchema.required(),
            password: passwordSchema
        }),
        onSubmit: async (values: FormLoginProps) => {
            console.log('login', await AccountsApi.Login({
                username: values.email,
                password: values.password,
                returnUrl: queryStrings.ReturnUrl as string
            }) );
            console.log(JSON.stringify(values));
        }
    });

    return (
        <>
          <form onSubmit={handleSubmit}>
              <label htmlFor="email">Email</label>
              <input id="email" name="email" type="text" autoComplete="on" onChange={handleChange}/>
              <Error message={errors.email} />
              <label htmlFor="password">Password</label>
              <input id="password" name="password" type="password" autoComplete="on" onChange={handleChange}/>
              <Error message={errors.password} />
              <button type="submit">Login</button>
          </form>
        </>
    );
};
