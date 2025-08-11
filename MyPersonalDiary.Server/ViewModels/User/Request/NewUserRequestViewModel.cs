using MyPersonalDiary.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.ViewModels.User.Request
{
    public class NewUserRequestViewModel
    {
        [Required(ErrorMessage = "Nickname is required.")]
        [StringLength(50, ErrorMessage = "Nickname cannot be longer than 50 characters.")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters and at most 100 characters.")]
        public string Password { get; set; }

        public UserRole? Role { get; set; }
    }
}
