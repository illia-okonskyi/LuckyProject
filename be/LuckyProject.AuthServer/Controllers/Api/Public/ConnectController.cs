using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.OpenId.Responses;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Controllers;
using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Web.Extensions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.IO;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LuckyProject.AuthServer.Controllers.Api.Public
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ConnectController : LpApiControllerBase
    {
        #region Internals & ctor
        private readonly IOpenIdService openIdService;
        private readonly IUsersService usersService;
        private readonly IUsersLoginService usersLoginService;
        private readonly IAuthSessionManager authSessionManager;
        private readonly IStringService stringService;
        private readonly ILogger logger;

        public ConnectController(
            IOpenIdService openIdService,
            IUsersService usersService,
            IUsersLoginService usersLoginService,
            IAuthSessionManager authSessionManager,
            IStringService stringService,
            ILogger<ConnectController> logger)
        {
            this.openIdService = openIdService;
            this.usersService = usersService;
            this.usersLoginService = usersLoginService;
            this.authSessionManager = authSessionManager;
            this.stringService = stringService;
            this.logger = logger;
        }
        #endregion

        #region Requests/Responses
        public class WebAuthorizeChallengeVerifyRequest
        {
            public string ClientId { get; set; }
            public Guid? UserId { get; set; }
            public string UserNameOrEmail { get; set; }
            public string Password { get; set; }
        }

        public class WebAuthorizeChallengeVerifyResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error,
                TwoFactor
            }

            public ResultType Result { get; set; }
            public Guid? UserId { get; set; }
            public Guid? TwoFactorRequestId { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class WebAuthorizeChallengeTwoFactorResendRequest
        {
            public string ClientId { get; set; }
            public Guid UserId { get; set; }
            public Guid TwoFactorRequestId { get; set; }
        }

        public class WebAuthorizeChallengeTwoFactorResendResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error
            }

            public ResultType Result { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class WebAuthorizeChallengeTwoFactorVerifyRequest
        {
            public string ClientId { get; set; }
            public Guid UserId { get; set; }
            public Guid TwoFactorRequestId { get; set; }
            public string TwoFactorCode { get; set; }
        }

        public class WebAuthorizeChallengeTwoFactorVerifyResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error
            }

            public ResultType Result { get; set; }
            public Guid? UserId { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class ForgotPasswordRequest
        {
            public string UserNameOrEmail { get; set; }
        }

        public class ForgotPasswordResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error
            }

            public ResultType Result { get; set; }
            public Guid RequestId { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class ResetPasswordRequest
        {
            public Guid RequestId { get; set; }
            public string Password { get; set; }
            public string PasswordRepeat { get; set; }
            public string Code { get; set; }
        }

        public class ResetPasswordCodeResendRequest
        {
            public Guid RequestId { get; set; }
        }

        public class ResetPasswordCodeResendResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error
            }

            public ResultType Result { get; set; }
            public string ErrorMessage { get; set; }
        }

        public class ResetPasswordResponse
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public enum ResultType
            {
                Success,
                Error
            }

            public ResultType Result { get; set; }
            public string ErrorMessage { get; set; }
        }
        #endregion

        #region Web Client Authorization
        #region WebAuthorizeStartAsync
        [HttpGet]
        [HttpPost]
        [Route("authorize-start")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public async Task<IActionResult> WebAuthorizeStartAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = HttpContext.GetOpenIddictServerRequest();
                if (request == null)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidRequest,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest");
                }

                if (request.HasPrompt(OpenIddictConstants.Prompts.None))
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.ConsentRequired,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.consentRequired");
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client?.Type != LpApiClientType.Web)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidClient,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient");
                }

                return await WebAuthorizeChallengeAsync(request, client, cancellationToken);

            }
            catch (OperationCanceledException)
            {
                return DefaultForbid(OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in WebAuthorizeStartAsync");
                return DefaultForbid(
                    OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.internal");
            }
        }
        #endregion

        #region WebAuthorizeChallengeVerifyAsync
        [HttpPost]
        [Route("authorize-challenge-verify")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<WebAuthorizeChallengeVerifyResponse>>
            WebAuthorizeChallengeVerifyAsync(
                WebAuthorizeChallengeVerifyRequest request,
                CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ClientId))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest"
                    });
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client?.Type != LpApiClientType.Web)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient"
                    });
                }

                if ((request?.UserId).HasValue)
                {
                    var user = await TryGetUserAsync(request.UserId.Value, cancellationToken);
                    if (user == null)
                    {
                        return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                        {
                            Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                            ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                        });
                    }

                    if (user.IsMachineUser)
                    {
                        return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                        {
                            Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                            ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                        });
                    }

                    if (!await usersLoginService.CanLoginAsync(user.Id))
                    {
                        return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                        {
                            Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                            ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                        });
                    }

                    var authorizations = await openIdService.GetUserAuthorizationsAsync(
                          client.Id,
                          user.Id,
                          cancellationToken);
                    if (authorizations.Count == 0)
                    {
                        return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                        {
                            Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                            ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.authExpired"
                        });
                    }

                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Success,
                        UserId = user.Id,
                    });
                }

                if (string.IsNullOrEmpty(request?.UserNameOrEmail) ||
                    string.IsNullOrEmpty(request?.Password))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest"
                    });
                }

                var verifyPasswordResponse = await usersLoginService.VerifyPasswordAsync(
                    request.UserNameOrEmail,
                    request.Password);
                if (verifyPasswordResponse.IsLockedOut)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                    });
                }

                if (!verifyPasswordResponse.CredsValid)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                    });
                }

                if (!verifyPasswordResponse.TwoFactorRequestId.HasValue)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                    {
                        Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.twoFactorFailed"
                    });
                }

                return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                {
                    Result = WebAuthorizeChallengeVerifyResponse.ResultType.TwoFactor,
                    UserId = verifyPasswordResponse.User.Id,
                    TwoFactorRequestId = verifyPasswordResponse.TwoFactorRequestId,
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new WebAuthorizeChallengeVerifyResponse
                {
                    Result = WebAuthorizeChallengeVerifyResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion

        #region WebAuthorizeChallengeTwoFactorResendAsync
        [HttpPost]
        [Route("authorize-challenge-two-factor-resend")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<WebAuthorizeChallengeTwoFactorResendResponse>>
            WebAuthorizeChallengeTwoFactorResendAsync(
                WebAuthorizeChallengeTwoFactorResendRequest request,
                CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ClientId))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest"
                    });
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client?.Type != LpApiClientType.Web)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient"
                    });
                }

                var user = await TryGetUserAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                    });

                }

                if (!await usersLoginService.CanLoginAsync(user.Id))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                    });
                }

                var resendResponse = await usersLoginService.RequestTwoFactorCodeAgainAsync(
                    request.TwoFactorRequestId);
                if (!resendResponse)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.twoFactorFailed"
                    });
                }

                return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                {
                    Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Success,
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorResendResponse
                {
                    Result = WebAuthorizeChallengeTwoFactorResendResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion

        #region WebAuthorizeChallengeTwoFactorVerifyAsync
        [HttpPost]
        [Route("authorize-challenge-two-factor-verify")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<WebAuthorizeChallengeTwoFactorVerifyResponse>>
            WebAuthorizeChallengeTwoFactorVerifyAsync(
                WebAuthorizeChallengeTwoFactorVerifyRequest request,
                CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.ClientId))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest"
                    });
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client?.Type != LpApiClientType.Web)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient"
                    });
                }

                var user = await TryGetUserAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                    });

                }

                if (!await usersLoginService.CanLoginAsync(user.Id))
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                    });
                }

                var loginResponse = await usersLoginService.LoginAsync(
                    request.TwoFactorRequestId,
                    request.TwoFactorCode);
                if (loginResponse.IsLockedOut)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                    });
                }

                if (!loginResponse.Success)
                {
                    return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                    {
                        Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                    });
                }

                return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                {
                    Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Success,
                    UserId = loginResponse.User.Id,
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new WebAuthorizeChallengeTwoFactorVerifyResponse
                {
                    Result = WebAuthorizeChallengeTwoFactorVerifyResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion

        #region WebAuthorizeFinishAsync
        [HttpGet]
        [HttpPost]
        [Route("authorize-finish")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public async Task<IActionResult> WebAuthorizeFinishAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = HttpContext.GetOpenIddictServerRequest();
                if (request == null)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidRequest,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest");
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client?.Type != LpApiClientType.Web)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidClient,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient");
                }

                var query = await GetQueryFromHttpContext(cancellationToken);
                var hasCancel = TryGetQueryFirstValue(
                    query,
                    "lp_cancel",
                    out string cancel);
                if (hasCancel && cancel == "yes")
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.ServerError,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled");
                }

                var user = await WebAuthorizeFinishAsync_TryGetUserFromQuery(
                    query,
                    cancellationToken);
                if (user == null)
                {
                    return await WebAuthorizeChallengeAsync(
                        request,
                        client,
                        cancellationToken);
                }

                if (!await usersLoginService.CanLoginAsync(user.Id))
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidGrant,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.tokenExpired");
                }

                var hasSessionId = TryGetQueryFirstValue(
                    query,
                    "lp_sid",
                    out Guid sessionId);
                var hasConsentResult = TryGetQueryFirstValue(
                    query,
                    "lp_cr",
                    out string consentResult);
                var authorizations = await openIdService.GetUserAuthorizationsAsync(
                      client.Id,
                      user.Id,
                      cancellationToken);
                var hasAuthorizations = authorizations.Count > 0;
                if (!hasConsentResult && hasAuthorizations)
                {
                    return await DefaultSignInAsync(
                        client,
                        user,
                        hasSessionId ? sessionId : null,
                        request.GetScopes(),
                        authorizations,
                        cancellationToken);
                }

                if (!hasConsentResult)
                {
                    return await WebAuthorizeFinishAsync_Consent(
                        request,
                        client,
                        user,
                        cancellationToken);
                }

                if (consentResult != "yes")
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.AccessDenied,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.signInReject");
                }

                return await DefaultSignInAsync(
                    client,
                    user,
                    hasSessionId ? sessionId : null,
                    request.GetScopes(),
                    authorizations,
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in WebAuthorizeFinishAsync");
                return DefaultForbid(OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.internal");
            }
        }

        private async Task<UserResponse> WebAuthorizeFinishAsync_TryGetUserFromQuery(
            IQueryCollection query,
            CancellationToken cancellationToken)
        {
            if (!TryGetQueryFirstValue(query, "lp_uid", out Guid userId))
            {
                return null;
            }

            return await TryGetUserAsync(userId, cancellationToken);
        }

        private async Task<IActionResult> WebAuthorizeFinishAsync_Consent(
             OpenIddictRequest request,
             ClientResponse client,
             UserResponse user,
             CancellationToken cancellationToken)
        {
            var query = await GetQueryFromHttpContext(cancellationToken);
            var hasSessionId = TryGetQueryFirstValue(
                query,
                "lp_sid",
                out Guid sessionId);
            var queryString = BuildQueryString(
                query,
                request.GetPrompts(),
                client.DisplayName,
                user.Id,
                hasSessionId ? sessionId : null,
                user.UserName,
                user.FullName);
            return FrontEndRedirect("consent" + queryString);
        }
        #endregion

        #region WebLogout
        #region WebLogoutStartAsync
        [HttpGet]
        [HttpPost]
        [Route("logout-start")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public async Task<IActionResult> WebLogoutStartAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var (actionResult, client, user) = await WebLogoutEnsureRequestAsync(
                    cancellationToken);
                if (actionResult != null)
                {
                    return actionResult;
                }

                var query = await GetQueryFromHttpContext(cancellationToken);
                var queryString = BuildQueryString(
                    query,
                    clientDisplayName: client.DisplayName,
                    userFullName: user.FullName);
                return FrontEndRedirect("logout" + queryString);
            }
            catch (OperationCanceledException)
            {
                return await WebLogoutErrorAsync(
                    "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in WebLogoutStartAsync");
                return await WebLogoutErrorAsync(
                    "lp-authserver-errors:s.lp.authserver.fe.errors.internal",
                    cancellationToken);
            }
        }
        #endregion

        #region WebLogoutFinishAsync
        [HttpGet]
        [HttpPost]
        [Route("logout-finish")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        public async Task<IActionResult> WebLogoutFinishAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var (actionResult, client, user) = await WebLogoutEnsureRequestAsync(
                    cancellationToken);
                if (actionResult != null)
                {
                    return actionResult;
                }

                var query = await GetQueryFromHttpContext(cancellationToken);
                if (!TryGetQueryFirstValue(query, "lp_sid", out Guid sessionId))
                {
                    return await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest",
                        cancellationToken);
                }

                if (!TryGetQueryFirstValue(query, "lp_lr", out string logoutResult))
                {
                    return await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest",
                        cancellationToken);
                }

                if (logoutResult != "full" && logoutResult != "app")
                {
                    return await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest",
                        cancellationToken);
                }

                if (logoutResult == "app")
                {
                    await authSessionManager.OnUserLoggedOutAppAsync(sessionId, cancellationToken);
                    return SignOut(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new() { RedirectUri = "/" });
                }

                await usersLoginService.LogoutAsync();
                await authSessionManager.OnUserLoggedOutFullOrDeletedAsync(user.Id, cancellationToken);
                await openIdService.RevokeUserAuthorizationsAsync(
                    client.Id,
                    user.Id,
                    cancellationToken);
                return SignOut(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new() { RedirectUri = "/" });
            }
            catch (OperationCanceledException)
            {
                return await WebLogoutErrorAsync(
                    "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in WebLogoutFinishAsync");
                return await WebLogoutErrorAsync(
                    "lp-authserver-errors:s.lp.authserver.fe.errors.internal",
                    cancellationToken);
            }
        }
        #endregion

        #region WebLogoutErrorAsync
        private async Task<IActionResult> WebLogoutErrorAsync(
            string message,
            CancellationToken cancellationToken)
        {
            var query = await GetQueryFromHttpContext(cancellationToken);
            if (TryGetQueryFirstValue(query, "lp_ler", out string logoutErrorRedirect))
            {
                var logoutErrorRedirectQueryString = new QueryBuilder { { "lp_err", message } }
                    .ToQueryString()
                    .ToString();
                return Redirect(logoutErrorRedirect + logoutErrorRedirectQueryString);
            }

            var queryString = BuildQueryString(query, errorMessage: message);
            return FrontEndRedirect("logout" + queryString);
        }
        #endregion

        #region WebLogoutEnsureRequestAsync
        private async Task<(IActionResult, ClientResponse, UserResponse)>
             WebLogoutEnsureRequestAsync(CancellationToken cancellationToken)
        {
            var query = await GetQueryFromHttpContext(cancellationToken);
            var hasClientId = TryGetQueryFirstValue(query, "lp_cid", out string clientId);
            var hasUserId = TryGetQueryFirstValue(query, "lp_uid", out Guid userId);
            if (!(hasClientId && hasUserId))
            {
                return (
                    await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest",
                        cancellationToken),
                    null,
                    null);
            }


            var client = await TryGetClientAsync(clientId, cancellationToken);
            if (client?.Type != LpApiClientType.Web)
            {
                return (
                    await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient",
                        cancellationToken),
                    null,
                    null);
            }

            var user = await TryGetUserAsync(userId, cancellationToken);
            if (user == null)
            {
                return (
                    await WebLogoutErrorAsync(
                        "lp-authserver-errors:s.lp.authserver.fe.errors.accessDenied",
                        cancellationToken),
                    null,
                    null);
            }

            return (null, client, user);
        }
        #endregion
        #endregion
        #endregion

        #region Forgot Password
        #region ForgotPasswordAsync
        [HttpPost]
        [Route("forgot-password")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<ForgotPasswordResponse>> ForgotPasswordAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.UserNameOrEmail))
                {
                    return LpApiResponse.Create(new ForgotPasswordResponse
                    {
                        Result = ForgotPasswordResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest"
                    });
                }

                var forgotPasswordResponse = await usersLoginService.ForgotPasswordAsync(
                    request.UserNameOrEmail);
                if (!forgotPasswordResponse.UserFound)
                {
                    return LpApiResponse.Create(new ForgotPasswordResponse
                    {
                        Result = ForgotPasswordResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds"
                    });
                }

                if (forgotPasswordResponse.IsLockedOut)
                {
                    return LpApiResponse.Create(new ForgotPasswordResponse
                    {
                        Result = ForgotPasswordResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut"
                    });
                }

                if (!forgotPasswordResponse.RequestId.HasValue)
                {
                    return LpApiResponse.Create(new ForgotPasswordResponse
                    {
                        Result = ForgotPasswordResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.twoFactorFailed"
                    });
                }

                return LpApiResponse.Create(new ForgotPasswordResponse
                {
                    Result = ForgotPasswordResponse.ResultType.Success,
                    RequestId = forgotPasswordResponse.RequestId.Value,
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new ForgotPasswordResponse
                {
                    Result = ForgotPasswordResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion

        #region ResetPasswordCodeResendAsync
        [HttpPost]
        [Route("reset-password-code-resend")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<ResetPasswordCodeResendResponse>>
            ResetPasswordCodeResendAsync(
                ResetPasswordCodeResendRequest request,
                CancellationToken cancellationToken)
        {
            try
            {
                var resendResponse = await usersLoginService.RequestResetPasswordCodeAgainAsync(
                    request.RequestId);
                if (!resendResponse)
                {
                    return LpApiResponse.Create(new ResetPasswordCodeResendResponse
                    {
                        Result = ResetPasswordCodeResendResponse.ResultType.Error,
                        ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.twoFactorFailed"
                    });
                }

                return LpApiResponse.Create(new ResetPasswordCodeResendResponse
                {
                    Result = ResetPasswordCodeResendResponse.ResultType.Success,
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new ResetPasswordCodeResendResponse
                {
                    Result = ResetPasswordCodeResendResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion

        #region ResetPasswordAsync
        [HttpPost]
        [Route("reset-password")]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<LpApiResponse<ResetPasswordResponse>> ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var resetPasswordResponse = await usersLoginService.ResetPasswordAsync(
                    request.RequestId,
                    request.Password,
                    request.PasswordRepeat,
                    request.Code);
                if (resetPasswordResponse == Services.Users.Responses.ResetPasswordResponse.Success)
                {
                    return LpApiResponse.Create(new ResetPasswordResponse
                    {
                        Result = ResetPasswordResponse.ResultType.Success
                    });
                }

                var errorMessage = resetPasswordResponse switch
                {
                    Services.Users.Responses.ResetPasswordResponse.NotRequested =>
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds",
                    Services.Users.Responses.ResetPasswordResponse.InvalidCode =>
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidCreds",
                    Services.Users.Responses.ResetPasswordResponse.PasswordsMistmatch =>
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidPassword",
                    Services.Users.Responses.ResetPasswordResponse.InvalidPassword =>
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidPassword",
                    Services.Users.Responses.ResetPasswordResponse.UserLockedOut =>
                        "lp-authserver-errors:s.lp.authserver.fe.errors.userLockedOut",
                    _ => "lp-authserver-errors:s.lp.authserver.fe.errors.internal"
                };
                return LpApiResponse.Create(new ResetPasswordResponse
                {
                    Result = ResetPasswordResponse.ResultType.Error,
                    ErrorMessage = errorMessage
                });
            }
            catch (OperationCanceledException)
            {
                return LpApiResponse.Create(new ResetPasswordResponse
                {
                    Result = ResetPasswordResponse.ResultType.Error,
                    ErrorMessage = "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled"
                });
            }
        }
        #endregion
        #endregion

        #region TokenAsync
        [HttpPost]
        [HttpGet]
        [Route("token")]
        [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> TokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = HttpContext.GetOpenIddictServerRequest();
                if (request == null)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidRequest,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidRequest");
                }

                var client = await TryGetClientAsync(request.ClientId, cancellationToken);
                if (client == null)
                {
                    return DefaultForbid(
                        OpenIddictConstants.Errors.InvalidClient,
                        "lp-authserver-errors:s.lp.authserver.fe.errors.invalidClient");
                }

                var sessionId = await TokenAsync_TryGetSessionId(cancellationToken);

                return client.Type == LpApiClientType.Web
                    ? await TokenAsync_Web(request, client, sessionId, cancellationToken)
                    : await TokenAsync_Machine(request, client, sessionId, cancellationToken);

            }
            catch (OperationCanceledException)
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in TokenAsync");
                return DefaultForbid(
                    OpenIddictConstants.Errors.ServerError,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.internal");
            }
        }

        private async Task<Guid?> TokenAsync_TryGetSessionId(
            CancellationToken cancellationToken)
        {
            try
            {
                var identity = await HttpContext
                    .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (!identity.Succeeded)
                {
                    return null;
                }

                if (!Guid.TryParse(
                    identity.Principal.GetClaim(LpWebConstants.Auth.Jwt.Claims.SessionId),
                    out var sessionId))
                {
                    return null;
                }

                return sessionId;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private async Task<IActionResult> TokenAsync_Web(
            OpenIddictRequest request,
            ClientResponse client,
            Guid? sessionId,
            CancellationToken cancellationToken)
        {
            if (!(request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType()))
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.InvalidGrant,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.invalidGrant");
            }

            var user = await TokenAsync_Web_TryGetUser(cancellationToken);
            if (user == null)
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.AccessDenied,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.accessDenied");
            }

            if (!await usersLoginService.CanLoginAsync(user.Id))
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.InvalidGrant,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.tokenExpired");
            }

            var authorizations = await openIdService.GetUserAuthorizationsAsync(
                  client.Id,
                  user.Id,
                  cancellationToken);
            var hasConsentPrompt = request.HasPrompt(OpenIddictConstants.Prompts.Consent);
            if (authorizations.Count == 0 || hasConsentPrompt)
            {
                return await WebAuthorizeChallengeAsync(request, client, cancellationToken);
            }

            return await DefaultSignInAsync(
                client,
                user,
                sessionId,
                request.GetScopes(),
                authorizations,
                cancellationToken);
        }

        private async Task<UserResponse> TokenAsync_Web_TryGetUser(
            CancellationToken cancellationToken)
        {
            try
            {
                var identity = await HttpContext
                    .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                if (!identity.Succeeded)
                {
                    return null;
                }

                if (!Guid.TryParse(
                    identity.Principal.GetClaim(OpenIddictConstants.Claims.Subject),
                    out var userId))
                {
                    return null;
                }

                return await TryGetUserAsync(userId, cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<IActionResult> TokenAsync_Machine(
             OpenIddictRequest request,
             ClientResponse client,
             Guid? sessionId,
             CancellationToken cancellationToken)
        {
            if (!(request.IsClientCredentialsGrantType() || request.IsRefreshTokenGrantType()))
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.InvalidGrant,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.invalidGrant");
            }

            UserResponse user = null;
            var lpIdentity = HttpContext.User.GetLpIdentity();
            if (lpIdentity != null)
            {
                user = await TryGetUserAsync(lpIdentity.UserId, cancellationToken);
            }
            else
            {
                var userName = usersService.GetMachineUserName(
                    openIdService.GetClientName(client.ClientId));
                user = await usersService.GetUserByUserNameAsync(userName);
            }

            if (user == null)
            {
                return DefaultForbid(
                    OpenIddictConstants.Errors.AccessDenied,
                    "lp-authserver-errors:s.lp.authserver.fe.errors.accessDenied");
            }

            var scopes = request.GetScopes();
            var authorizations = await openIdService.GetUserAuthorizationsAsync(
                client.Id,
                user.Id,
                cancellationToken);
            return await DefaultSignInAsync(
                client,
                user,
                sessionId,
                scopes,
                authorizations,
                cancellationToken);
        }
        #endregion

        #region Internals
        #region Helpers
        private async Task<ClientResponse> TryGetClientAsync(
            string clientId,
            CancellationToken cancellationToken)
        {
            try
            {
                return await openIdService.GetClientByClientIdAsync(clientId, cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<UserResponse> TryGetUserAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            try
            {
                return await usersService.GetUserByIdAsync(userId, cancellationToken);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<IQueryCollection> GetQueryFromHttpContext(
             CancellationToken cancellationToken)
        {
            var request = HttpContext.Request;
            if (request.Method == HttpMethods.Get)
            {
                return request.Query;
            }

            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadLineAsync(cancellationToken);
            request.Body.Position = 0;
            return new QueryCollection(stringService.GetQueryParams(body));
        }

        private string BuildQueryString(
            IQueryCollection query,
            IEnumerable<string> prompts = null,
            string clientDisplayName = null,
            Guid? userId = null,
            Guid? sessionId = null,
            string userName = null,
            string userFullName = null,
            string errorMessage = null)
        {
            prompts ??= Enumerable.Empty<string>();
            var builder = new QueryBuilder(query
                .Where(kvp => kvp.Key != OpenIddictConstants.Parameters.Prompt &&
                    kvp.Key != "lp_err")
                .ToList());
            prompts = prompts
                .Except([OpenIddictConstants.Prompts.Login], StringComparer.Ordinal)
                .ToList();
            if (prompts.SafeCount() > 0)
            {
                builder.Add(
                    OpenIddictConstants.Parameters.Prompt,
                    string.Join(" ", prompts));
            }

            if (!query.ContainsKey("lp_cdn") && !string.IsNullOrEmpty(clientDisplayName))
            {
                builder.Add("lp_cdn", clientDisplayName);
            }

            if (!query.ContainsKey("lp_uid") && userId.HasValue)
            {
                builder.Add("lp_uid", userId.Value.ToString());
            }

            if (!query.ContainsKey("lp_sid") && sessionId.HasValue)
            {
                builder.Add("lp_sid", sessionId.Value.ToString());
            }

            if (!query.ContainsKey("lp_un") && !string.IsNullOrEmpty(userName))
            {
                builder.Add("lp_un", userName);
            }

            if (!query.ContainsKey("lp_ufn") && !string.IsNullOrEmpty(userFullName))
            {
                builder.Add("lp_ufn", userFullName);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                builder.Add("lp_err", errorMessage);
            }

            return builder.ToQueryString().ToString();
        }

        private bool TryGetQueryFirstValue(IQueryCollection query, string key, out string value)
        {
            value = null;
            if (!query.TryGetValue(key, out var sv))
            {
                return false;
            }
            value = sv.First();
            return true;
        }

        private bool TryGetQueryFirstValue(IQueryCollection query, string key, out Guid value)
        {
            value = default;
            if (!TryGetQueryFirstValue(query, key, out string valueString))
            {
                return false;
            }

            if (!Guid.TryParse(valueString, out var guid))
            {
                return false;
            }

            value = guid;
            return true;
        }
        #endregion

        #region Handlers
        private IActionResult DefaultForbid(string error, string description)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string>
                {
                    { OpenIddictServerAspNetCoreConstants.Properties.Error, error },
                    { OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription, description }
                }));
        }

        private async Task<IActionResult> WebAuthorizeChallengeAsync(
             OpenIddictRequest request,
             ClientResponse client,
             CancellationToken cancellationToken)
        {
            var query = await GetQueryFromHttpContext(cancellationToken);
            var hasSessionId = TryGetQueryFirstValue(
                query,
                "lp_sid",
                out Guid sessionId);
            var queryString = BuildQueryString(
                query,
                request.GetPrompts(),
                client.DisplayName,
                null,
                hasSessionId ? sessionId : null,
                null,
                null);
            return FrontEndRedirect("challenge" + queryString);
        }

        private IActionResult FrontEndRedirect(string path)
        {
            return LocalRedirect("/#/" + path);
        }

        private async Task<IActionResult> DefaultSignInAsync(
            ClientResponse client,
            UserResponse user,
            Guid? sessionId,
            IEnumerable<string> scopes,
            List<UserAuthorizationResponse> authorizations,
            CancellationToken cancellationToken)
        {
            var signedInSessionId = await authSessionManager.OnUserSignedInAsync(
                client.Id.ToString(),
                user.Id,
                sessionId,
                cancellationToken);
            var roleIds = await usersService.GetUserRoleIdsAsync(user.Id, cancellationToken);
            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: LpWebConstants.Auth.Jwt.Claims.UserName,
                roleType: LpWebConstants.Auth.Jwt.Claims.Role)
                .SetClaim(LpWebConstants.Auth.Jwt.Claims.Subject, user.Id.ToString())
                .SetClaim(LpWebConstants.Auth.Jwt.Claims.ClientId, client.Id.ToString())
                .SetClaim(LpWebConstants.Auth.Jwt.Claims.SessionId, signedInSessionId.ToString())
                .SetResources(LpWebConstants.Auth.Jwt.ApiAudience)
                .SetScopes(scopes);

            var authorization = authorizations.LastOrDefault();
            authorization ??= await openIdService.CreateUserAuthorizationAsync(
                client.Id,
                user.Id,
                identity,
                cancellationToken);
            identity.SetAuthorizationId(authorization.Id.ToString())
                .SetDestinations(GetClaimDestinations);

            return SignIn(
                new ClaimsPrincipal(identity),
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        #endregion

        #region GetClaimDestinations
        private static IEnumerable<string> GetClaimDestinations(Claim claim)
        {
            switch (claim.Type)
            {
                case LpWebConstants.Auth.Jwt.Claims.Subject:
                case LpWebConstants.Auth.Jwt.Claims.ClientId:
                case LpWebConstants.Auth.Jwt.Claims.SessionId:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;

                // NOTE: Never include the security stamp in the access and identity tokens,
                //       as it's a secret value.
                case "AspNet.Identity.SecurityStamp":
                    yield break;
                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
        #endregion
        #endregion
    }
}
