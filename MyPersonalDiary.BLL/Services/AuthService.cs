using AutoDependencyRegistration.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyPersonalDiary.BLL.DTOs.Auth.Request;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.Models.Identities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.BLL.Services
{
    [RegisterClassAsTransient]
    public class AuthService(IConfiguration configuration, IUserService userService, UserManager<ApplicationUser> userManager) : IAuthService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly IUserService _userService = userService;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<string> GenerateToken(string userId, string userEmail)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, userEmail),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            string userRole = await _userService.GetNameRoleByUserIdAsync(long.Parse(userId));
            var user = await _userManager.FindByIdAsync(userId) ?? throw new Exception("User not found");

            claims.Add(new Claim(ClaimTypes.Role, userRole));
            claims.Add(new Claim(ClaimTypes.Name, user.NickName ?? "N/A"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email)
                ?? throw new InvalidLoginEmailException("User by email not founded!");

            if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                throw new InvalidLoginPasswordException("Invalid password!");

            return true;
        }
    }
}
