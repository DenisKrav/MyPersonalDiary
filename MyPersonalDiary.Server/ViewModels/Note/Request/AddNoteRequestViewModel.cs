using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Server.ViewModels.Diary.Request
{
    public class AddNoteRequestViewModel
    {
        [MaxLength(500, ErrorMessage = "Content cannot exceed 500 characters.")]
        public string? Content { get; set; }

        public IFormFile? Image { get; set; }
    }
}
