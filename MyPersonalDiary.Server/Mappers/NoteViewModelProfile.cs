using AutoMapper;
using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.DTOs.User.Request;
using MyPersonalDiary.Server.ViewModels.Diary.Request;
using MyPersonalDiary.Server.ViewModels.Note.Request;
using MyPersonalDiary.Server.ViewModels.User.Request;

namespace MyPersonalDiary.Server.Mappers
{
    public class NoteViewModelProfile: Profile
    {
        public NoteViewModelProfile()
        {
            CreateMap<AddNoteRequestViewModel, AddNoteRequestDto>();
            CreateMap<DeleteNoteRequestViewModel, DeleteNoteRequestDto>();
        }
    }
}
