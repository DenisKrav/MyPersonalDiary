import { useMutation } from "@tanstack/react-query";
import { restoreAccount } from "../../Services/UserService";

export const useRestoreAccount = () => {
  return useMutation<void, Error, { email: string }>({
    mutationFn: ({ email }) => restoreAccount(email),
  });
};