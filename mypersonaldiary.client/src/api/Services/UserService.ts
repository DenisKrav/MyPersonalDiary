import axios from "axios";
import type { GeneralResultModel } from "../Models/GeneralResultModel/GeneralResultModel";

const API_BASE_URL = import.meta.env.VITE_ASPNETCORE_API_URL;

export const deleteUser = async (token: string): Promise<string> => {
    const res = await axios.delete<GeneralResultModel<string>>(
        `${API_BASE_URL}/api/User/DeleteUser`,
        {
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        }
    );

    const { result, errors, hasErrors } = res.data;
    if (hasErrors || !result) {
        throw new Error(errors?.join(", ") || "Failed to delete user");
    }
    return result;
};

export const restoreAccount = async (email: string): Promise<void> => {
  const response = await axios.post<GeneralResultModel<any>>(
    `${API_BASE_URL}/api/User/RestoreAccount`,
    { email },
    {
      headers: { "Content-Type": "application/json" },
    }
  );

  const { hasErrors, errors } = response.data;
  if (hasErrors) {
    throw new Error(errors?.join(", ") || "Restore failed");
  }
};