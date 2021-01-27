import { IdentityServerConfig } from "../config";
import axios from "axios";
import * as qs from 'query-string';

export const getIdentityToken = async () => {
    const { authority, tokenUrl, clientId, clientSecret, scope, grantType } = IdentityServerConfig;
    const endpoint = axios.create({
        baseURL: authority,
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/x-www-form-urlencoded'
        }
    });

    const response = await endpoint.post(
        tokenUrl,
        qs.stringify({
            scope: scope,
            client_id: clientId,
            client_secret: clientSecret,
            grant_type: grantType
        })
    );

    return response.data.access_token;
};
