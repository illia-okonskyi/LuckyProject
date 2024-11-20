using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Models;

namespace LuckyProject.Lib.Web.Exceptions
{
    public class LpWebException : LpException
    {
        public TrString ErrorMessage { get; }

        public LpWebException(string message, TrString errorMessage = null)
            : base($"{message} -> {errorMessage}")
        {
            ErrorMessage = errorMessage;
        }
    }

    public class LpBadRequestWebException : LpWebException
    {
        public LpBadRequestWebException(TrString errorMessage = null)
            : base("Bad Request", errorMessage)
        { }
    }

    public class LpBadGatewayWebException : LpWebException
    {
        public LpBadGatewayWebException(TrString errorMessage = null)
            : base("Bad Gateway", errorMessage)
        { }
    }

    public class LpServiceUnavailableWebException : LpWebException
    {
        public LpServiceUnavailableWebException(TrString errorMessage = null)
            : base("Service Unavailable", errorMessage)
        { }
    }

    public class LpGatewayTimeoutWebException : LpWebException
    {
        public LpGatewayTimeoutWebException(TrString errorMessage = null)
            : base("Gateway Timeout", errorMessage)
        { }
    }
}
