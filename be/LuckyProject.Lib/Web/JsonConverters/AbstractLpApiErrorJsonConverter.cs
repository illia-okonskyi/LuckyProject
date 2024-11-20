using LuckyProject.Lib.Basics.JsonConverters;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using Newtonsoft.Json.Linq;
using System;

namespace LuckyProject.Lib.Web.JsonConverters
{
    public class AbstractLpApiErrorJsonConverter : AbstractJsonCreationConverter<AbstractLpApiError>
    {
        protected override AbstractLpApiError Create(Type objectType, JObject jObject)
        {
            var type = jObject[nameof(AbstractLpApiError.Type)].Value<string>();
            return type switch
            {
                LpWebConstants.Errors.Client.BadRequest.ErrorType =>
                    new LpBadRequestApiError(),
                LpWebConstants.Errors.Client.ValidationError.ErrorType =>
                    new LpValidationErrorApiError(),
                LpWebConstants.Errors.Client.Unauthorized.ErrorType =>
                    new LpUnauthorizedApiError(),
                LpWebConstants.Errors.Client.InvalidCredentials.ErrorType =>
                    new LpInvalidCredentialsApiError(),
                LpWebConstants.Errors.Client.AccessDenied.ErrorType =>
                    new LpAccessDeniedApiError(),
                LpWebConstants.Errors.Server.InternalServerError.ErrorType =>
                    new LpInternalServerErrorApiError(),
                LpWebConstants.Errors.Server.NotImplemented.ErrorType =>
                    new LpNotImplementedApiError(),
                LpWebConstants.Errors.Server.BadGateway.ErrorType =>
                    new LpBadGatewayApiError(),
                LpWebConstants.Errors.Server.ServiceUnavailable.ErrorType =>
                    new LpServiceUnavailableApiError(),
                LpWebConstants.Errors.Server.GatewayTimeout.ErrorType =>
                    new LpGatewayTimeoutApiError(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
