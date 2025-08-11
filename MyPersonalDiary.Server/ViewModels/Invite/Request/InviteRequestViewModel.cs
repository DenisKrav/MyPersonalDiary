using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.ViewModels.Invite.Request
{
    public class InviteRequestViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }
    }
}
