using AutoMapper;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.Server.ViewModels.User.Request;

namespace MyPersonalDiary.Server.Mappers
{
    public class UserViewModelProfile: Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<NewUserRequestViewModel, NewUserDTO>();
        }
    }
}
