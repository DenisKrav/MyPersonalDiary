using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Exceptions
{
    public class InvalidLoginEmailException : Exception
    {
        public InvalidLoginEmailException() : base() { }

        public InvalidLoginEmailException(string message) : base(message) { }

        public InvalidLoginEmailException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
