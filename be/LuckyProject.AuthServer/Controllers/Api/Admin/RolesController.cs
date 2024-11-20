using LuckyProject.AuthServer.Services.Roles;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.AuthServer.Services.Users.Responses;
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
    public class RolesController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private readonly IRolesService rolesService;

        public RolesController(IRolesService rolesService)
        {
            this.rolesService = rolesService;
        }
        #endregion

        #region Create
        public class CreateRequest
        {
            public string Name { get; init; }
            public string Description { get; init; }
        }

        [HttpPost]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<RoleResponse>> CreateRoleAsync(
            CreateRequest request,
            CancellationToken cancellationToken = default)
        {
            var role = await rolesService.CreateRoleAsync(new()
            {
                Name = request.Name,
                Description = request.Description,
            });
            return LpApiResponse.Create(role);
        }

        #endregion

        #region Get
        [HttpPost]
        [Route("list")]
        public async Task<LpApiResponse<PaginatedList<RoleResponse>>> GetRolesAsync(
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var roles = await rolesService.GetRolesAsync(
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(roles);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<LpApiResponse<RoleResponse>> GetRoleAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var role = await rolesService.GetRoleByIdAsync(id, cancellationToken);
            return LpApiResponse.Create(role);
        }
        #endregion

        #region Update
        public class UpdateRequest : CreateRequest
        {
            public Guid Id { get; init; }
        }

        [HttpPut]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<RoleResponse>> UpdateRoleAsync(
            UpdateRequest request)
        {
            var role = await rolesService.UpdateRoleAsync(new()
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description
            });
            return LpApiResponse.Create(role);
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Route("{id}")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeleteRoleAsync(Guid id)
        {
            await rolesService.DeleteRoleAsync(id);
            return LpApiResponse.Create();
        }
        #endregion

        #region Permissions
        [HttpPost]
        [Route("{id}/permissions")]
        public async Task<LpApiResponse<PaginatedList<RolePermissionsResponse>>>
            GetRolePermissionsAsync(
                Guid id,
                LpFilterOrderPaginationRequest request,
                CancellationToken cancellationToken = default)
        {
            var permissions = await rolesService.GetRolePermissionsAsync(
                id,
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(permissions);
        }

        public class RolePermissionRequest
        {
            public Guid RoleId { get; init; }
            public Guid PermissionId { get; init; }
        }

        [HttpPost]
        [Route("assign-permission")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> AssignPermissionToRoleAsync(
            RolePermissionRequest request)
        {
            await rolesService.AssignPermissionToRoleAsync(new()
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            });
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("delete-permission")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeletePermissionFromRoleAsync(
            RolePermissionRequest request)
        {
            await rolesService.DeletePermissionFromRoleAsync(new()
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            });
            return LpApiResponse.Create();
        }
        #endregion

        #region Users
        [HttpPost]
        [Route("{id}/users")]
        public async Task<LpApiResponse<PaginatedList<UserResponse>>> GetRoleUsersAsync(
            Guid id,
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var users = await rolesService.GetRoleUsersAsync(
                id,
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(users);
        }

        public class RoleUserRequest
        {
            public Guid RoleId { get; init; }
            public Guid UserId { get; init; }
        }

        [HttpPost]
        [Route("assign-user")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> AssignUserToRoleAsync(RoleUserRequest request)
        {
            await rolesService.AssignUserToRoleAsync(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("delete-user")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeleteUserFromRoleAsync(RoleUserRequest request)
        {
            await rolesService.DeleteUserFromRoleAsync(new()
            {
                RoleId = request.RoleId,
                UserId = request.UserId
            });
            return LpApiResponse.Create();
        }
        #endregion
    }
}
