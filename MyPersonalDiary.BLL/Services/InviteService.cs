using AutoDependencyRegistration.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyPersonalDiary.BLL.Exceptions;
using MyPersonalDiary.BLL.InterfacesServices;
using MyPersonalDiary.DAL.ApplicationDbContext;
using MyPersonalDiary.DAL.InterfacesRepositories;
using MyPersonalDiary.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Services
{
    [RegisterClassAsTransient]
    public class InviteService : IInviteService
    {
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        public InviteService(IConfiguration config, IUnitOfWork unitOfWork, IUserService userService)
        {
            _config = config;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<string> SendInviteAsync(string email)
        {
            // Checking for possible errors and inaccuracies in the database
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            var userExists = await _userService.GetUserByEmailAsync(email);
            if (userExists != null)
                throw new ArgumentException("User with this email already exists.");

            var existingInvites = await _unitOfWork.InviteRepository
                .GetAsync(i => i.Email == email && !i.IsUsed);
            if (existingInvites.Any())
                foreach (var existingInvite in existingInvites)
                {
                    existingInvite.IsUsed = true;
                    existingInvite.UsedAt = DateTime.UtcNow;
                    await _unitOfWork.InviteRepository.UpdateAsync(existingInvite);
                }
            await _unitOfWork.SaveAsync();

            // Send email
            var code = Guid.NewGuid().ToString("N").Substring(0, 8);

            var resOfSending = await SendEmailAsync(email, code);

            if (!resOfSending)
                throw new SendEmailExeption("Failed to send email. Please check your SMTP configuration.");

            // Save invite to the database
            var invite = new Invite
            {
                Email = email,
                Code = code,
                SentAt = DateTime.UtcNow
            };

            await _unitOfWork.InviteRepository.InsertAsync(invite);
            await _unitOfWork.SaveAsync();

            return "Invite sent successfully.";
        }

        private async Task<bool> SendEmailAsync(string email, string code)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = _config["Email:Smtp:Host"],
                    Port = int.Parse(_config["Email:Smtp:Port"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        _config["Email:Smtp:Username"],
                        _config["Email:Smtp:Password"]
                    )
                };

                var frontendBaseUrl = _config["BaseUrl"];
                var inviteLink = $"{frontendBaseUrl}/invite?code={code}";
                var from = _config["Email:Smtp:From"];
                var message = new MailMessage(from, email)
                {
                    Subject = "Your invite",
                    Body = $"Your invitation link: {inviteLink}"
                };

                await smtp.SendMailAsync(message);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
