using Azure.Core;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Middleware;
using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Web.Models.AuthorizationAuthentication;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Middleware
{
    public class AuthServerJwtAuthorizationAuthenticationHandler
        : ILpJwtAuthorizationAuthenticationHandler
    {
        private readonly IServiceScopeService scopeService;

        public AuthServerJwtAuthorizationAuthenticationHandler(IServiceScopeService scopeService)
        {
            this.scopeService = scopeService;
        }

        public async Task<LpIdentity> HasAccessAsync(
            string origin,
            string token,
            List<LpAuthRequirement> requirements)
        {
            using var scope = scopeService.CreateScope();
            var openIdService = scope.ServiceProvider.GetRequiredService<IOpenIdService>();
            var (tokenValid, identity) = await openIdService.ValidateAccessTokenAsync(token);
            if (!tokenValid)
            {
                return null;
            }

            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var authorizeResponse = await usersService.AuthorizeAsync(
                requirements,
                identity.UserId);
            if (authorizeResponse == null)
            {
                throw new LpAccessDeniedAuthException(new("User has no acccess"));
            }

            identity.UserName = authorizeResponse.User.UserName;
            identity.UserEmail = authorizeResponse.User.Email;
            identity.UserPhoneNumber = authorizeResponse.User.PhoneNumber;
            identity.UserFullName = authorizeResponse.User.FullName;
            identity.UserTelegramUserName = authorizeResponse.User.TelegramUserName;
            identity.UserPreferredLocale = authorizeResponse.User.PreferredLocale;
            identity.UserRoleIds = authorizeResponse.UserRoleIds;
            return identity;
        }
    }
}
