namespace MyPersonalDiary.Server.ViewModels.Diary.Request
{
    public class AddNoteRequestViewModel
    {
        public string? Content { get; set; }
        public IFormFile? Image { get; set; }
    }
}
