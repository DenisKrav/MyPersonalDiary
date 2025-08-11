using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPersonalDiary.BLL.Exceptions
{
    public class UserExistExeption : Exception
    {
        public UserExistExeption() : base() { }

        public UserExistExeption(string message) : base(message) { }

        public UserExistExeption(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
