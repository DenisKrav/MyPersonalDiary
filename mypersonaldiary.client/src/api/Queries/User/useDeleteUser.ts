import { useMutation } from "@tanstack/react-query";
import { deleteUser } from "../../Services/UserService";

export const useDeleteUser = (token: string) => {
  return useMutation<string, Error>({
    mutationFn: () => deleteUser(token),
  });
};