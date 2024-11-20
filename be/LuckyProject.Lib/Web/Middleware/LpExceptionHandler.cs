using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Exceptions;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Middleware
{
    public class LpExceptionHandler : IExceptionHandler
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly IJsonService jsonService;
        private readonly ILogger logger;

        public LpExceptionHandler(
            IJsonService jsonService,
            ILogger<LpExceptionHandler> logger)
        {
            this.jsonService = jsonService;
            this.logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            (int statusCode, AbstractLpApiError error, bool isCritical) =
                CreateExceptionDetails(exception, httpContext);
            if (isCritical)
            {
                logger.LogError(exception, "Critical Unhandled Exception");
            }
            else
            {
                logger.LogWarning(exception, "NonCritical Unhandled Exception");
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(
                jsonService.SerializeObject(LpApiResponse.Create(error), JsonSerializerSettings),
                cancellationToken);
            return true;
        }

        private static (int StatusCode, AbstractLpApiError Error, bool IsCritical)
            CreateExceptionDetails(Exception ex, HttpContext httpContext)
        {
            var controller = httpContext
                .Request
                .RouteValues
                .TryGetValue("controller", out var controllerO)
                    ? controllerO.ToString()
                    : "<NONE>";
            var action = httpContext
                .Request
                .RouteValues
                .TryGetValue("action", out var actionO)
                    ? actionO.ToString()
                    : "<NONE>";

            if (ex is LpBadRequestWebException badRequest)
            {
                return (
                    LpWebConstants.Errors.Client.BadRequest.StatusCode,
                    new LpBadRequestApiError(badRequest.ErrorMessage),
                    false);
            }

            if (ex is LpValidationErrorException validationError)
            {
                return (
                    LpWebConstants.Errors.Client.ValidationError.StatusCode,
                    new LpValidationErrorApiError(validationError.Result),
                    false);
            }

            if (ex is LpUnathorizedAuthException unathorized)
            {
                return (
                    LpWebConstants.Errors.Client.Unauthorized.StatusCode,
                    new LpUnauthorizedApiError(unathorized.ErrorMessage),
                    false);
            }

            if (ex is LpInvalidCredentialsAuthException invalidCreds)
            {
                return (
                    LpWebConstants.Errors.Client.InvalidCredentials.StatusCode,
                    new LpInvalidCredentialsApiError(invalidCreds.ErrorMessage),
                    false);
            }

            if (ex is LpAccessDeniedAuthException accessDenied)
            {
                return (
                    LpWebConstants.Errors.Client.AccessDenied.StatusCode,
                    new LpAccessDeniedApiError(accessDenied.ErrorMessage),
                    false);
            }

            if (ex is NotImplementedException)
            {
                return (
                    LpWebConstants.Errors.Server.NotImplemented.StatusCode,
                    new LpNotImplementedApiError(
                        new TrString(
                            "Action Not Implemented: {0}/{1}",
                            new() { controller, action })),
                    true);
            }

            if (ex is LpBadGatewayWebException badGateway)
            {
                return (
                    LpWebConstants.Errors.Server.BadGateway.StatusCode,
                    new LpBadGatewayApiError(badGateway.ErrorMessage),
                    false);
            }

            if (ex is LpServiceUnavailableWebException serviceUnavailable)
            {
                return (
                    LpWebConstants.Errors.Server.ServiceUnavailable.StatusCode,
                    new LpServiceUnavailableApiError(serviceUnavailable.ErrorMessage),
                    false);
            }

            if (ex is LpGatewayTimeoutWebException gatewayTimeout)
            {
                return (
                    LpWebConstants.Errors.Server.GatewayTimeout.StatusCode,
                    new LpGatewayTimeoutApiError(gatewayTimeout.ErrorMessage),
                    false);
            }

            // NOTE: Fallback, internal server error
            return (
                LpWebConstants.Errors.Server.InternalServerError.StatusCode,
                new LpInternalServerErrorApiError(
                    new TrString("Something went wrong: {0}", new() { ex.Message })),
                true);
        }
    }
}
