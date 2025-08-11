using AutoDependencyRegistration.Attributes;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.DTOs.User.Response;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Services
{
    [RegisterClassAsTransient]
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager, 
            RoleManager<ApplicationRole> roleManager, 
            IMapper mapper, 
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(IdentityResult, long)> CreateUserAsync(NewUserDTO newUserDTO)
        {
            if (string.IsNullOrWhiteSpace(newUserDTO.Email) || string.IsNullOrWhiteSpace(newUserDTO.Password))
                throw new ArgumentException("Email and password cannot be empty.");

            var userExist = await _userManager.FindByEmailAsync(newUserDTO.Email);
            if (userExist != null)
                throw new ArgumentException("User with this email already exists.");

            var user = _mapper.Map<ApplicationUser>(newUserDTO);
            user.UserName = user.Email;

            string role = newUserDTO.Role.ToString();

            var result = await _userManager.CreateAsync(user, newUserDTO.Password);
            if (!result.Succeeded)
                return (result, user.Id);

            await _userManager.AddToRoleAsync(user, role);
            return (result, 0);
        }

        public async Task<IdentityResult> DeleteUserAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> DeleteSoftUserAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            user.IsPendingDeletion = true;
            user.DeletionRequestedAt = DateTime.UtcNow;

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<int> PurgeSoftDeletedUsersAsync(TimeSpan olderThan)
        {
            var cutoff = DateTime.UtcNow - olderThan;

            var usersToDelete = await _userManager.Users
                .Where(u => u.IsPendingDeletion && u.DeletionRequestedAt != null && u.DeletionRequestedAt <= cutoff)
                .ToListAsync();

            var deleted = 0;

            foreach (var user in usersToDelete)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    deleted++;
                    _logger.LogInformation("Permanently deleted user {Email}", user.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to delete user {Email}: {Errors}",
                        user.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            return deleted;
        }

        public async Task<IdentityResult> RestoreSoftDeletedUserAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!user.IsPendingDeletion && user.LockoutEnabled && user.DeletionRequestedAt == null && user.LockoutEnd == null)
            {
                throw new UserExistExeption("User is not pending deletion or already restored.");
            }

            user.IsPendingDeletion = false;
            user.DeletionRequestedAt = null;

            user.LockoutEnabled = false;
            user.LockoutEnd = null;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(UpdateUserDTO updateUserDTO)
        {
            var user = await _userManager.FindByIdAsync(updateUserDTO.UserId.ToString());
            if (user == null)
                throw new ArgumentException("User not found");

            _mapper.Map(updateUserDTO, user);

            return await _userManager.UpdateAsync(user);
        }

        public async Task<UserDTO?> GetUserByIdAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> GetUserByPhoneAsync(string phone)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<string> GetNameRoleByUserIdAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new UserArgumentException("User not found!");

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault();

            return currentRole ?? "N/A";
        }
    }
}
