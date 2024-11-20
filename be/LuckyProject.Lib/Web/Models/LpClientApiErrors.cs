using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Constants;

namespace LuckyProject.Lib.Web.Models
{
    public class LpBadRequestApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpBadRequestApiError()
            : base(LpWebConstants.Errors.Client.BadRequest.ErrorType, true)
        { }

        public LpBadRequestApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpValidationErrorApiError : AbstractLpApiError
    {
        public LpValidationResult Result { get; init; }

        public LpValidationErrorApiError()
            : base(LpWebConstants.Errors.Client.ValidationError.ErrorType, true)
        { }

        public LpValidationErrorApiError(LpValidationResult result)
            : this()
        {
            Result = result;
        }
    }

    public class LpUnauthorizedApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpUnauthorizedApiError()
            : base(LpWebConstants.Errors.Client.Unauthorized.ErrorType, true)
        { }

        public LpUnauthorizedApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpInvalidCredentialsApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpInvalidCredentialsApiError()
            : base(LpWebConstants.Errors.Client.InvalidCredentials.ErrorType, true)
        { }

        public LpInvalidCredentialsApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }

    public class LpAccessDeniedApiError : AbstractLpApiError
    {
        public TrString Message { get; init; }

        public LpAccessDeniedApiError()
            : base(LpWebConstants.Errors.Client.AccessDenied.ErrorType, true)
        { }

        public LpAccessDeniedApiError(TrString message)
            : this()
        {
            Message = message;
        }
    }
}
