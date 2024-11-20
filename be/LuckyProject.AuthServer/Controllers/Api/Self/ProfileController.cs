using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Web.Attributes;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mime;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.AuthServer.Services.LpApi;

namespace LuckyProject.AuthServer.Controllers.Api.Self
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [LpJwtAuthorize("bAuthServerApi")]
    public class ProfileController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private readonly ILpLocalizationService localizationService;
        private readonly IUsersService usersService;
        private readonly IUsersLoginService usersLoginService;
        private readonly ILpApiService apiService;

        public ProfileController(
            ILpLocalizationService localizationService,
            IUsersService usersService,
            IUsersLoginService usersLoginService,
            ILpApiService apiService)
        {
            this.localizationService = localizationService;
            this.usersService = usersService;
            this.usersLoginService = usersLoginService;
            this.apiService = apiService;
        }
        #endregion

        #region GetProfileAsync
        public class ProfileResponse
        {
            public class ApiResponse
            {
                public string Name { get; init; }
                public string Endpoint { get; init; }
                public List<string> Features { get; init; }
            }

            public Guid Id { get; init; }
            public Guid SessionId { get; init; }
            public string UserName { get; init; }
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string FullName { get; init; }
            public string TelegramUserName { get; init; }
            public string PreferredLocale { get; init; }
            public List<Guid> RoleIds { get; init; }
            public List<ApiResponse> Apis { get; init; } = new();
        }

        [HttpGet]
        public async Task<LpApiResponse<ProfileResponse>> GetProfileAsync(
            CancellationToken cancellationToken = default)
        {
            var identity = GetLpIdentity();
            var apiFeatures = await apiService.GetUserFeaturesAsync(identity.UserId, cancellationToken);

            return LpApiResponse.Create(new ProfileResponse
            {
                Id = identity.UserId,
                SessionId = identity.SessionId,
                UserName = identity.UserName,
                Email = identity.UserEmail,
                PhoneNumber = identity.UserPhoneNumber,
                FullName = identity.UserFullName,
                TelegramUserName = identity.UserTelegramUserName,
                PreferredLocale = identity.UserPreferredLocale,
                RoleIds = identity.UserRoleIds.ToList(),
                Apis = apiFeatures.Apis.Select(a => new ProfileResponse.ApiResponse
                {
                    Name = a.Name,
                    Endpoint = a.Endpoint,
                    Features = a.Features,
                })
                .ToList()
            });
        }
        #endregion

        #region GetLocales
        [HttpGet]
        [Route("locales")]
        public LpApiResponse<List<LpLocaleInfo>> GetLocales()
        {
            return LpApiResponse.Create(localizationService
                .GetDefaultLocales()
                .Where(l => !string.IsNullOrEmpty(l.Name))
                .ToList());
        }
        #endregion

        #region UpdateProfileAsync
        public class UpdateProfileRequest
        {
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string FullName { get; init; }
            public string TelegramUserName { get; init; }
            public string PreferredLocale { get; init; }
        }

        [HttpPut]
        public async Task<LpApiResponse<ProfileResponse>> UpdateProfileAsync(
            UpdateProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            var identity = GetLpIdentity();
            if (identity.ClientType != LpApiClientType.Web)
            {
                throw new LpAccessDeniedAuthException();
            }

            var user = await usersService.UpdateWebUserAsync(new()
            {
                Id = identity.UserId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                TelegramUserName = request.TelegramUserName,
                PreferredLocale = request.PreferredLocale,
            });
            return LpApiResponse.Create(new ProfileResponse
            {
                Id = user.Id,
                SessionId = identity.SessionId,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                TelegramUserName = user.TelegramUserName,
                PreferredLocale = user.PreferredLocale,
                RoleIds = identity.UserRoleIds.ToList()
            });
        }
        #endregion

        #region UpdatePasswordAsync
        public class UpdatePasswordRequest
        {
            public string OldPassword { get; init; }
            public string NewPassword { get; init; }
            public string NewPasswordRepeat { get; init; }
        }

        [Route("password")]
        [HttpPut]
        public async Task<LpApiResponse> UpdatePasswordAsync(
            UpdatePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            var identity = GetLpIdentity();
            if (identity.ClientType != LpApiClientType.Web)
            {
                throw new LpAccessDeniedAuthException();
            }

            var oldPasswordValid = await usersLoginService.CheckPasswordAsync(new()
            {
                Id = identity.UserId,
                Password = request.OldPassword
            });

            if (!oldPasswordValid)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(request.OldPassword),
                    ValidationErrorCodes.Credentials);
            }

            await usersLoginService.ResetPasswordAsync(new()
            {
                Id = identity.UserId,
                NewPassword = request.NewPassword,
                NewPasswordRepeat = request.NewPasswordRepeat
            });
            return LpApiResponse.Create();
        }
        #endregion
    }
}
