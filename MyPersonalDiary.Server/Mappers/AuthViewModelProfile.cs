using AutoMapper;
using MyPersonalDiary.BLL.DTOs.Auth.Request;
using MyPersonalDiary.Server.ViewModels.Auth.Request;

namespace MyPersonalDiary.Server.Mappers
{
    public class AuthViewModelProfile: Profile
    {
        public AuthViewModelProfile()
        {
            CreateMap<LoginRequestViewModel, LoginRequestDto>();
        }
    }
}
