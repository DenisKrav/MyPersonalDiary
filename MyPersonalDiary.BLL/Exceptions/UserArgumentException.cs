using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Exceptions
{
    public class UserArgumentException: Exception
    {
        public UserArgumentException() : base() { }

        public UserArgumentException(string message) : base(message) { }

        public UserArgumentException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
