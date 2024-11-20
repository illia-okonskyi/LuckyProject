using OpenIddict.Abstractions;
using System.Net;

namespace LuckyProject.Lib.Web.Constants
{
    public static class LpWebConstants
    {
        public static class Cors
        {
            public const string DynamicPolicyName = "LpDynamicCorsPolicy";
        }

        public static class ApiClients
        {
            public const string WebPrefix = "web-";
            public const string MachinePrefix = "m2m-";
        }

        public static class Auth
        {
            public static class Jwt
            {
                public const string TokenType = "Bearer";
                public const string AuthenticationScheme = "LpJWT";
                public const string AuthoriaztionScheme = "LpJWT";
                public const string AuthoriaztionPolicyPrefix = "LpJWT-Policy:";
                public const string ApiAudience = "lp-api";

                public static class Claims
                {
                    public const string ClaimsIssuer = "LuckyProject-AuthServer";

                    public const string Subject = OpenIddictConstants.Claims.Subject;
                    public const string ClientId = OpenIddictConstants.Claims.ClientId;
                    public const string SessionId = "session_id";
                    public const string UserName = OpenIddictConstants.Claims.Username;
                    public const string Email = OpenIddictConstants.Claims.Email;
                    public const string PhoneNumber = OpenIddictConstants.Claims.PhoneNumber;
                    public const string FullName = "full_name";
                    public const string TelegramUserName = "tg_username";
                    public const string PreferredLocale = "pref_locale";
                    public const string Role = OpenIddictConstants.Claims.Role;
                }
            }
        }

        public static class Internals
        {
            public static class HttpClients
            {
                public const string DefaultJwtAuthorizationAuthenticationHandler =
                    "LpHttpClient.DefaultJwtAuthorizationAuthenticationHandler";
                public const string ApiCallback = "LpHttpClient.ApiCallback";
            }
        }

        #region LpApiErrors
        public static class Errors
        {
            #region Client errors
            public static class Client
            {
                public static class BadRequest
                {
                    public const int StatusCode = (int)HttpStatusCode.BadRequest;
                    public const string ErrorType = "bad-request";
                }

                public static class ValidationError
                {
                    public const int StatusCode = (int)HttpStatusCode.BadRequest;
                    public const string ErrorType = "validation-error";
                }

                public static class Unauthorized
                {
                    public const int StatusCode = (int)HttpStatusCode.Unauthorized;
                    public const string ErrorType = "unauthorized";
                }

                public static class InvalidCredentials
                {
                    public const int StatusCode = (int)HttpStatusCode.Unauthorized;
                    public const string ErrorType = "invalid-credentials";
                }

                public static class AccessDenied
                {
                    public const int StatusCode = (int)HttpStatusCode.Forbidden;
                    public const string ErrorType = "access-denied";
                }
            }
            #endregion

            #region Server errors
            public static class Server
            {
                public static class InternalServerError
                {
                    public const int StatusCode = (int)HttpStatusCode.InternalServerError;
                    public const string ErrorType = "internal-server-error";
                }

                public static class NotImplemented
                {
                    public const int StatusCode = (int)HttpStatusCode.NotImplemented;
                    public const string ErrorType = "not-implemented";
                }

                public static class BadGateway
                {
                    public const int StatusCode = (int)HttpStatusCode.BadGateway;
                    public const string ErrorType = "bad-gateway";
                }

                public static class ServiceUnavailable
                {
                    public const int StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    public const string ErrorType = "service-unavailable";
                }

                public static class GatewayTimeout
                {
                    public const int StatusCode = (int)HttpStatusCode.GatewayTimeout;
                    public const string ErrorType = "gateway-timeout";
                }
            }
            #endregion
        }
        #endregion
    }
}
