import { Home } from './Home';
import { Login } from './Login';

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
    }
];
