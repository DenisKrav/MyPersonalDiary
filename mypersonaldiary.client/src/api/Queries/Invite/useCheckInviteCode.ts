import { useMutation } from '@tanstack/react-query';
import { validateUserInvite } from '../../Services/InviteUserService';
import type { InviteCodeModel } from '../../Models/Invete/InviteCodeModel';

export const useCheckInviteCode = () => {
    return useMutation<string, Error, InviteCodeModel>({
        mutationFn: (invite) => {
            return validateUserInvite(invite);
        },
    });
};