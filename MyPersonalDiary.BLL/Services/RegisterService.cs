using AutoDependencyRegistration.Attributes;
using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.InterfacesRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Services
{
    [RegisterClassAsTransient]
    public class RegisterService: IRegisterService
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterService(IUserService userService, IUnitOfWork unitOfWork) 
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task RegisterUserAsync(NewUserDTO newUser)
        {
            var userWithThisEmail = await _userService.GetUserByEmailAsync(newUser.Email);

            if (userWithThisEmail != null)
            {
                throw new UserArgumentException("User with this email alraedy exist!");
            }

            await _userService.CreateUserAsync(newUser);

            // Make invate used
            var invites = await _unitOfWork.InviteRepository.GetAsync(i => i.Email == newUser.Email && !i.IsUsed);
            var invite = invites.FirstOrDefault();
            if (invite != null)
            {
                invite.IsUsed = true;
                invite.UsedAt = DateTime.UtcNow;
                await _unitOfWork.InviteRepository.UpdateAsync(invite);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
