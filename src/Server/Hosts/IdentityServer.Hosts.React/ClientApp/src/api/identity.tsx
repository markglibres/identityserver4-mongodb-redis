import axios from "axios";
import {ILoginForm} from "../types/Login";
import {ILogoutForm} from "../types";

const {post} = axios.create({
    baseURL: 'https://localhost:5001/identity'
});

export const login: ({...props}: ILoginForm) => Promise<any> = async ({...props} : ILoginForm)  => {
    const {data} = await post(
        'account/login',
        {
            username: props.Username,
            password: props.Password,
            returnUrl: props.ReturnUrl
        },
    );

    return data;
};

export const logout: ({...props}: ILogoutForm) => Promise<any> = async ({...props} : ILogoutForm)  => {
    console.log('props', props);
    const {data} = await post(
        'account/logout',
        props,
    );
    return data;
};

