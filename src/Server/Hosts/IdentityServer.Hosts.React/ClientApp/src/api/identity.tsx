import axios from "axios";
import {ILoginForm} from "../types/Login";

const apiEndpoint = axios.create({
    baseURL: 'https://localhost:5001/identity'
});

const login: ({...props}: ILoginForm) => Promise<any> = async ({...props} : ILoginForm)  => {
    var response = await apiEndpoint.post(
        'account/login',
        {
            username: props.Username,
            password: props.Password,
            returnUrl: props.ReturnUrl
        },
    );

    return response.data;
};

export { login };
