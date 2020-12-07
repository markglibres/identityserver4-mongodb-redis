import React from 'react';
import {useFormik} from "formik";
import {Error} from "../design-system/atoms/Error";
import * as Yup from 'yup';
import {emailAddressSchema, passwordSchema} from "../validations/schema";

interface FormLoginProps {
    email: string;
    password: string
}
const formLoginInitialValues = {
    email: '',
    password: ''
}

export const Login: React.FC = () => {

    const { errors, handleSubmit, handleChange } = useFormik<FormLoginProps>({
        initialValues: formLoginInitialValues,
        validationSchema: Yup.object({
            email: emailAddressSchema.required(),
            password: passwordSchema
        }),
        onSubmit: (values: FormLoginProps) => {
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
