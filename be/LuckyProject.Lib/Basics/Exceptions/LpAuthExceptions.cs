using LuckyProject.Lib.Basics.Models;

namespace LuckyProject.Lib.Basics.Exceptions
{
    public class LpAuthException : LpException
    {
        public TrString ErrorMessage { get; }

        public LpAuthException(string message, TrString errorMessage = null)
            : base($"{message} -> {errorMessage}")
        {
            ErrorMessage = errorMessage;
        }
    }

    public class LpUnathorizedAuthException : LpAuthException
    {
        public LpUnathorizedAuthException(TrString errorMessage = null)
            : base("Unathorized", errorMessage)
        { }
    }

    public class LpInvalidCredentialsAuthException : LpAuthException
    {
        public LpInvalidCredentialsAuthException(TrString errorMessage = null)
            : base("Invalid Credentials", errorMessage)
        { }
    }

    public class LpAccessDeniedAuthException : LpAuthException
    {
        public LpAccessDeniedAuthException(TrString errorMessage = null)
            : base("Access Denied", errorMessage)
        { }
    }
}
