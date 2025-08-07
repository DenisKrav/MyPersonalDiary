import axios from "axios";
import type { GeneralResultModel } from "../Models/GeneralResultModel/GeneralResultModel";
import type { InviteModel } from "../Models/Invete/InviteModel";
import type { InviteCodeModel } from "../Models/Invete/InviteCodeModel";

const API_BASE_URL = import.meta.env.VITE_ASPNETCORE_API_URL;

export const inviteUser = async (invite: InviteModel, token: string): Promise<string> => {
    const response = await axios.post<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/Invite/SendInvite`,
        invite,
        {
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
        }
    );

    if (!response.data.result) {
        throw new Error("Token is missing");
    }

    return response.data.result;
};

export const validateUserInvite = async (inviteCode: InviteCodeModel): Promise<string> => {
    const response = await axios.post<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/Invite/ValidateCode`,
        inviteCode,
        {
            headers: {
                'Content-Type': 'application/json'
            },
        }
    );

    if (!response.data.result) {
        throw new Error("Token is missing");
    }

    return response.data.result;
};