import { useMutation } from '@tanstack/react-query';
import type { InviteModel } from '../../Models/Invete/InviteModel';
import { inviteUser } from '../../Services/InviteUserService';
import { useAuth } from '../../../context/AuthContext';

export const useInviteUser = () => {
    const { token } = useAuth();

    return useMutation<string, Error, InviteModel>({
        mutationFn: (invite) => {
            if (!token) {
                return Promise.reject(new Error("User is not authenticated"));
            }

            return inviteUser(invite, token);
        },
    });
};