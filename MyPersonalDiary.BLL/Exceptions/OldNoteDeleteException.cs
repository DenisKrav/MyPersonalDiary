using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Exceptions
{
    public class OldNoteDeleteException: Exception
    {
        public OldNoteDeleteException() : base() { }

        public OldNoteDeleteException(string message) : base(message) { }

        public OldNoteDeleteException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
