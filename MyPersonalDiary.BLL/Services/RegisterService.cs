using AutoDependencyRegistration.Attributes;
using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
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

        public RegisterService(IUserService userService) 
        {
            _userService = userService;
        }

        public async Task RegisterUserAsync(NewUserDTO newUser)
        {
            var userWithThisEmail = await _userService.GetUserByEmailAsync(newUser.Email);

            if (userWithThisEmail != null)
            {
                throw new UserArgumentException("User with this email alraedy exist!");
            }

            await _userService.CreateUserAsync(newUser);
        }
    }
}
