using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.DTOs.Note.Request
{
    public class DeleteNoteRequestDto
    {
        public string NoteType { get; set; }
        public Guid NoteId { get; set; }
    }
}
