using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using LuckyProject.Lib.Web.Constants;
using System;
using Microsoft.Extensions.Logging;
using LuckyProject.Lib.Basics.Extensions;

namespace LuckyProject.Lib.Web.Middleware
{
    public interface ILpJwtAuthorizationAuthenticationHandler
    {
        /// <summary>
        /// Must return not null identity if has access
        /// </summary>
        Task<LpIdentity> HasAccessAsync(
            string origin,
            string token,
            List<LpAuthRequirement> requirements);
    }

    public class LpJwtAuthorizationHandler: AuthorizationHandler<LpJwtAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILpJwtAuthorizationAuthenticationHandler authHandler;
        private readonly ILogger logger;

        public LpJwtAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ILpJwtAuthorizationAuthenticationHandler authHandler,
            ILogger<LpJwtAuthorizationHandler> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.authHandler = authHandler;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            LpJwtAuthorizationRequirement requirement)
        {
            var headers = httpContextAccessor
                .HttpContext
                .Request
                .Headers;
            var origin = headers.Origin.ToString().NullIfEmpty();
            var authorization = headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authorization))
            {
                return;
            }

            var parts = authorization.Split(' ');
            if (parts.Length != 2)
            {
                return;
            }

            var tokenType = parts[0];
            if (!tokenType.Equals(LpWebConstants.Auth.Jwt.TokenType))
            {
                return;
            }
            var token = parts[1];
            try
            {
                var lpIdentity = await authHandler.HasAccessAsync(
                    origin,
                    token,
                    requirement.Requirements);
                if (lpIdentity == null)
                {
                    return;
                }

                context.User.AddIdentity(lpIdentity.ToClaimsIdentity());
                context.Succeed(requirement);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Exception in HandleRequirementAsync");
                return;
            }
        }
    }
}
