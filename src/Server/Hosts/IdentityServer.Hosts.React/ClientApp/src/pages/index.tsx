import { Home } from './Home';
import { Login } from './Login';
import {Logout} from "./Logout";

export const pages = [
    {
        props: {
            path: '/',
            component: Home,
            exact: true
        }
    },
    {
        props: {
            path: '/Account/Login',
            component: Login,
            exact: false
        }
    },
    {
        props: {
            path: '/Account/Logout',
            component: Logout,
            exact: false
        }
    }
];
