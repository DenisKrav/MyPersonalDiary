import { useMutation } from '@tanstack/react-query';
import type { NewUserModel } from '../../Models/User/NewUserModel';
import { registerUser } from '../../Services/RegisterService';

export const useRegisterUser = () => {
  return useMutation<string, Error, NewUserModel>({
    mutationFn: registerUser,
  });
};
