import axios from "axios";
import type { NewUserModel } from "../Models/User/NewUserModel";
import type { GeneralResultModel } from "../Models/GeneralResultModel/GeneralResultModel";

const API_BASE_URL = import.meta.env.VITE_ASPNETCORE_API_URL;

export const registerUser = async (newCustomer: NewUserModel): Promise<string> => {
    const response = await axios.post<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/register/RegisterUser`,
        newCustomer,
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