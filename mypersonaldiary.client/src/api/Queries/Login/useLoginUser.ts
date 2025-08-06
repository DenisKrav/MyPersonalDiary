import { useMutation } from '@tanstack/react-query';
import type { LoginModel } from '../../Models/Login/LoginModel';
import { loginUser } from '../../Services/LoginService';

export const useLoginUser = () => {
  return useMutation<string, Error, LoginModel>({
    mutationFn: loginUser,
  });
};
