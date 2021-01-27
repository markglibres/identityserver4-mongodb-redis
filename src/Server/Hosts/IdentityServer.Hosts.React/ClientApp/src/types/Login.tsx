export interface ILoginQuery {
    ReturnUrl: string;
}

export interface ILoginForm {
    Username: string;
    Password: string;
    ReturnUrl?: string;
}
