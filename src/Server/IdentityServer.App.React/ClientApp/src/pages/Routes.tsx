import { Login } from './index';

export const Routes = [
    {
        props: {
            component: Login,
            path: '/account/login',
            exact: true
        },
        title: 'Login'
    }
];

