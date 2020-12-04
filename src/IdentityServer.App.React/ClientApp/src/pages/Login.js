import React, { useCallback } from 'react';
import { useFormik } from "formik";
import { ValidationSchema, emailAddressSchema, passwordSchema } from "../validations/schema";
import {Error} from "../design-system/atoms/Error";

export const Login = () => {

    const { errors, handleSubmit, handleChange } = useFormik({
        initialValues: {
            email: '',
            password: ''
        },
        validationSchema: ValidationSchema({
            email: emailAddressSchema.required('Email is required.'),
            password: passwordSchema
        }),
        onSubmit: values => {
            console.log(JSON.stringify(values));
        }
    });

    const getError = useCallback((key) => {
        return errors && errors[key];
    }, [errors]);

    return (
        <>
          <form onSubmit={handleSubmit}>
              <label htmlFor="email">Email</label>
              <input id="email" name="email" type="text" autoComplete="on" onChange={handleChange}/>
              <Error message={getError('email')} />
              <label htmlFor="password">Password</label>
              <input id="password" name="password" type="password" autoComplete="on" onChange={handleChange}/>
              <Error message={getError('password')} />
              <button type="submit">Login</button>
          </form>
        </>
    );
};
