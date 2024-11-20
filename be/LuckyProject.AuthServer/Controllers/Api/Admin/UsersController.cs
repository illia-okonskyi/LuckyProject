using LuckyProject.AuthServer.Services.Users;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Attributes;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [LpJwtAuthorize("bapi-Admin-Panel-Read")]
    public class UsersController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private readonly IUsersService usersService;
        private readonly IUsersLoginService usersLoginService;

        public UsersController(IUsersService usersService, IUsersLoginService usersLoginService)
        {
            this.usersService = usersService;
            this.usersLoginService = usersLoginService;
        }
        #endregion

        #region Create
        public class CreateRequest
        {
            public string UserName { get; init; }
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string FullName { get; init; }
            public string Password { get; init; }
            public string TelegramUserName { get; init; }
            public string PreferredLocale { get; init; }
        }

        [HttpPost]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<UserResponse>> CreateUserAsync(
            CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await usersService.CreateWebUserAsync(new()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                Password = request.Password,
                TelegramUserName = request.TelegramUserName,
                PreferredLocale = request.PreferredLocale
            });
            return LpApiResponse.Create(user);
        }

        #endregion

        #region Get
        [HttpPost]
        [Route("list")]
        public async Task<LpApiResponse<PaginatedList<UserResponse>>> GetUsersAsync(
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var users = await usersService.GetUsersAsync(
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(users);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<LpApiResponse<UserResponse>> GetUserAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var user = await usersService.GetUserByIdAsync(id, cancellationToken);
            return LpApiResponse.Create(user);
        }
        #endregion

        #region Update
        public class UpdateMachineRequest
        {
            public Guid Id { get; init; }
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string PreferredLocale { get; init; }

        }

        [HttpPut]
        [Route("machine")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<UserResponse>> UpdateMachineUserAsync(
            UpdateMachineRequest request)
        {
            var user = await usersService.UpdateMachineUserAsync(new()
            {
                Id = request.Id,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PreferredLocale = request.PreferredLocale
            });
            return LpApiResponse.Create(user);
        }

        public class UpdateWebRequest : UpdateMachineRequest
        {
            public string FullName { get; init; }
            public string TelegramUserName { get; init; }
        }

        [HttpPut]
        [Route("web")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<UserResponse>> UpdateWebUserAsync(
            UpdateWebRequest request)
        {
            var user = await usersService.UpdateWebUserAsync(new()
            {
                Id = request.Id,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                TelegramUserName = request.TelegramUserName,
                PreferredLocale = request.PreferredLocale
            });
            return LpApiResponse.Create(user);
        }

        [HttpPut]
        [Route("web-password")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> ResetWebUserPasswordAsync(ResetPasswordRequest request)
        {
            await usersLoginService.ResetPasswordAsync(request);
            return LpApiResponse.Create();
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Route("{id}")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeleteUserAsync(Guid id)
        {
            await usersService.DeleteUserAsync(id);
            return LpApiResponse.Create();
        }
        #endregion

        #region Roles
        [HttpPost]
        [Route("{id}/roles")]
        public async Task<LpApiResponse<PaginatedList<RoleResponse>>> GetUserRolesAsync(
            Guid id,
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var users = await usersService.GetUserRolesAsync(
                id,
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(users);
        }

        public class UserRoleRequest
        {
            public Guid UserId { get; init; }
            public Guid RoleId { get; init; }
        }

        [HttpPost]
        [Route("assign-role")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> AssignRoleToUserAsync(UserRoleRequest request)
        {
            await usersService.AssignRoleToUserAsync(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("delete-role")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeleteRoleFromUserAsync(UserRoleRequest request)
        {
            await usersService.DeleteRoleFromUserAsync(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            return LpApiResponse.Create();
        }
        #endregion

        #region Permissions
        [HttpPost]
        [Route("{id}/permissions")]
        public async Task<LpApiResponse<PaginatedList<UserPermissionsResponse>>>
            GetUserPermissionsAsync(
                Guid id,
                LpFilterOrderPaginationRequest request,
                CancellationToken cancellationToken = default)
        {
            var permissions = await usersService.GetUserPermissionsAsync(
                id,
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(permissions);
        }
        #endregion
    }
}
