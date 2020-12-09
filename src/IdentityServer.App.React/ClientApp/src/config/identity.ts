interface IdentityServerConfigProps {
    authority: string;
    tokenUrl: string;
    clientId: string;
    clientSecret: string;
    scope: string;
    grantType: string;
}

interface IdentityManagementConfigProps {
    baseUrl: string;
    accountsLoginUrl: string;
}

export const IdentityServerConfig = {
    authority: process.env.REACT_APP_IDENTITY_AUTHORITY,
    tokenUrl: process.env.REACT_APP_IDENTITY_TOKEN_URL,
    clientId: process.env.REACT_APP_IDENTITY_CLIENT_ID,
    clientSecret: process.env.REACT_APP_IDENTITY_CLIENT_SECRET,
    scope: process.env.REACT_APP_IDENTITY_SCOPE,
    grantType: process.env.REACT_APP_IDENTITY_GRANT_TYPE,
} as IdentityServerConfigProps;

export const IdentityManagementConfig = {
    baseUrl: process.env.REACT_APP_IDENTITY_MANAGEMENT_URL,
    accountsLoginUrl: process.env.REACT_APP_IDENTITY_MANAGEMENT_ACCOUNTS_LOGIN
} as IdentityManagementConfigProps;
