using MyPersonalDiary.BLL.DTOs.Auth.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.InterfacesServices
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequestDto loginRequest);

        Task<string> GenerateToken(string userId, string userEmail);
    }
}
