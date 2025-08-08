using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.DTOs.Note.Response
{
    public class NoteResponseDto
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string? Content { get; set; }
        public string? ImageContentType { get; set; }
        public long? ImageSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
