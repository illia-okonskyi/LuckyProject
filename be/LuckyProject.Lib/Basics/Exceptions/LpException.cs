using System;

namespace LuckyProject.Lib.Basics.Exceptions
{
    public class LpException : Exception
    {
        public LpException()
        { }

        public LpException(string message) : base(message)
        { }

        public LpException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
