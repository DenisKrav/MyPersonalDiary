using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.BLL.DTOs.User.Response;
using MyPersonalDiary.DAL.Models.Identities;

namespace MyPersonalDiary.BLL.Mappers
{
    public class UserDTOProfile: Profile
    {
        public UserDTOProfile()
        {
            CreateMap<NewUserDTO, ApplicationUser>();
            CreateMap<UpdateUserDTO, ApplicationUser>();
            CreateMap<ApplicationUser, UserDTO>();
        }
    }
}
