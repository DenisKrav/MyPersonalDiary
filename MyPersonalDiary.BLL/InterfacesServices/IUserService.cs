using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.DTOs.User.Response;

namespace MyPersonalDiary.BLL.InterfacesServices
{
    public interface IUserService
    {
        Task<(IdentityResult, long)> CreateUserAsync(NewUserDTO newUserDTO);

        Task<IdentityResult> DeleteUserAsync(long userId);

        Task<IdentityResult> DeleteSoftUserAsync(long userId);

        Task<int> PurgeSoftDeletedUsersAsync(TimeSpan olderThan);

        Task<IdentityResult> RestoreSoftDeletedUserAsync(string userEmail);

        Task<IdentityResult> UpdateUserAsync(UpdateUserDTO updateUserDTO);

        Task<UserDTO?> GetUserByIdAsync(long userId);

        Task<UserDTO?> GetUserByEmailAsync(string email);

        Task<UserDTO?> GetUserByPhoneAsync(string phone);

        Task<string> GetNameRoleByUserIdAsync(long userId);
    }
}
