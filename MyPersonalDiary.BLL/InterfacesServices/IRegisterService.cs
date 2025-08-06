using Microsoft.AspNetCore.Identity;
using MyPersonalDiary.BLL.DTOs.User.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.InterfacesServices
{
    public interface IRegisterService
    {
        Task RegisterUserAsync(NewUserDTO newCustomer);
    }
}
