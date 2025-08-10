using MyPersonalDiary.BLL.DTOs.Note.Request;
using MyPersonalDiary.BLL.DTOs.Note.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.InterfacesServices
{
    public interface INoteService
    {
        Task<NoteResponseDto> AddNoteAsync(AddNoteRequestDto addNote);
        Task<IEnumerable<NoteResponseDto>> GetUserNotesAsync(long userId);
        Task<Guid> DeleteNoteAsync(DeleteNoteRequestDto deleterequest);
    }
}
