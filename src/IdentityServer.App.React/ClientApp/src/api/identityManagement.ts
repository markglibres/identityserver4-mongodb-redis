import { getIdentityToken } from "./authorization";
import Axios from "axios";
import {IdentityManagementConfig} from "../config";

const managementApi = Axios.create({
    baseURL: IdentityManagementConfig.baseUrl,
    headers: {
        Accept: 'application/json',
        'Content-Type': 'application/json',
    }
});

getIdentityToken().then(token => managementApi.defaults.headers.common.Authorization = `Bearer ${token}`);

export const AccountsApi = {
    Login: async (data: any) => {
        const response = await managementApi.post(IdentityManagementConfig.accountsLoginUrl, data);
        return response.data;
    }
};
