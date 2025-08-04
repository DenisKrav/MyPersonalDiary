namespace MyPersonalDiary.BLL.Exceptions
{
    public class InvalidLoginPasswordException : Exception
    {
        public InvalidLoginPasswordException() : base() { }

        public InvalidLoginPasswordException(string message) : base(message) { }

        public InvalidLoginPasswordException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}
