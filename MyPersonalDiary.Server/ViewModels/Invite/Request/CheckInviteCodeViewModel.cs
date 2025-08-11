using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.ViewModels.Invite.Request
{
    public class CheckInviteCodeViewModel
    {
        [Required(ErrorMessage = "Invite code is required.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Invite code must be between 5 and 50 characters.")]
        public string Code { get; set; }
    }
}
