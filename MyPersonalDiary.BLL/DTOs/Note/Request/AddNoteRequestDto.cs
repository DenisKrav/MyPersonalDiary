using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.DTOs.Note.Request
{
    public class AddNoteRequestDto
    {
        public long UserId { get; set; }
        public string? Content { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageContentType { get; set; }
    }
}
