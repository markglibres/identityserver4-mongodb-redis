import * as Yup from 'yup';

export const emailAddressSchema = Yup.string().email('Please enter a valid email address.');
export const passwordSchema = Yup.string().required('Password is required');

