import axios from "axios";
import type { GeneralResultModel } from "../Models/GeneralResultModel/GeneralResultModel";
import type { LoginModel } from "../Models/Login/LoginModel";

const API_BASE_URL = import.meta.env.VITE_ASPNETCORE_API_URL;

export const loginUser = async (loginData: LoginModel): Promise<string> => {
    const response = await axios.post<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/auth/login`,
        loginData,
        {
            headers: {
                'Content-Type': 'application/json',
            },
        }
    );

    if (!response.data.result) {
        throw new Error("Token is missing");
    }

    return response.data.result;
};