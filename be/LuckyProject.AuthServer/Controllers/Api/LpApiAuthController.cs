using LuckyProject.AuthServer.Services.Cors;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Controllers;
using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Web.Models.AuthorizationAuthentication;
using LuckyProject.Lib.Web.Models.Requests;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class LpApiAuthController : LpApiControllerBase
    {
        private readonly IAuthServerCorsService corsService;
        private readonly IOpenIdService openIdService;
        private readonly IUsersService  usersService;

        public LpApiAuthController(
            IAuthServerCorsService corsService,
            IOpenIdService openIdService,
            IUsersService usersService)
        {
            this.corsService = corsService;
            this.openIdService = openIdService;
            this.usersService = usersService;
        }

        [HttpPost]
        public async Task<LpApiResponse<LpAuthResponse>> AuthAsync(
            LpAuthRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!await corsService.IsOriginAllowedAsync(request.Origin, cancellationToken))
            {
                throw new LpAccessDeniedAuthException(new("Invalid origin"));
            }

            var (tokenValid, identity) = await openIdService
                .ValidateAccessTokenAsync(request.Token);
            if (!tokenValid)
            {
                throw new LpAccessDeniedAuthException(new("Invalid token"));
            }

            var authorizeResponse = await usersService.AuthorizeAsync(
                request.Requirements,
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
            return LpApiResponse.Create(new LpAuthResponse { Identity = identity });
        }
    }
}
