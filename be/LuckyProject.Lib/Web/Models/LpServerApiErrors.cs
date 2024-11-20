using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Constants;

namespace LuckyProject.Lib.Web.Models
{
    public class LpInternalServerErrorApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpInternalServerErrorApiError()
            : base(LpWebConstants.Errors.Server.InternalServerError.ErrorType, false)
        { }

        public LpInternalServerErrorApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpNotImplementedApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpNotImplementedApiError()
            : base(LpWebConstants.Errors.Server.NotImplemented.ErrorType, false)
        { }

        public LpNotImplementedApiError(TrString message)
            : this()
        {
            Message = message;
        }

    }

    public class LpBadGatewayApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpBadGatewayApiError()
            : base(LpWebConstants.Errors.Server.BadGateway.ErrorType, false)
        { }

        public LpBadGatewayApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpServiceUnavailableApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpServiceUnavailableApiError()
            : base(LpWebConstants.Errors.Server.ServiceUnavailable.ErrorType, false)
        { }

        public LpServiceUnavailableApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpGatewayTimeoutApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpGatewayTimeoutApiError()
            : base(LpWebConstants.Errors.Server.BadGateway.ErrorType, false)
        { }

        public LpGatewayTimeoutApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }
}
