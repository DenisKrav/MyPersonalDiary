using MyPersonalDiary.DAL.Enums;

namespace MyPersonalDiary.Server.ViewModels.User.Request
{
    public class NewUserRequestViewModel
    {
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole? Role { get; set; }
    }
}
