using LuckyProject.AuthServer.Services.Permissions;
using LuckyProject.AuthServer.Services.Permissions.Responses;
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
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [LpJwtAuthorize("bapi-Admin-Panel-Read")]
    public class PermissionsController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private readonly IPermissionsService permissionsService;

        public PermissionsController(IPermissionsService permissionsService)
        {
            this.permissionsService = permissionsService;
        }
        #endregion

        #region Get
        [HttpPost]
        [Route("list")]
        public async Task<LpApiResponse<PaginatedList<PermissionResponse>>> GetPermissionsAsync(
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var permissions = await permissionsService.GetPermissionsAsync(
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(permissions);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<LpApiResponse<PermissionResponse>> GetPermissionAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var permission = await permissionsService.GetPermissionByIdAsync(id, cancellationToken);
            return LpApiResponse.Create(permission);
        }
        #endregion

        #region Update
        public class UpdateRequest
        {
            public Guid Id { get; init; }
            public string Description { get; init; }
            public bool? Allow { get; init; }
            public int? Level { get; init; }
            public HashSet<string> Passkeys { get; init; }
        }

        [HttpPut]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<PermissionResponse>> UpdatePermissionAsync(
            UpdateRequest request)
        {
            var permission = await permissionsService.UpdatePermissionAsync(new()
            {
                Id = request.Id,
                Description = request.Description,
                Allow = request.Allow,
                Level = request.Level,
                Passkeys = request.Passkeys
            });
            return LpApiResponse.Create(permission);
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Route("{id}")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeletePermissionAsync(Guid id)
        {
            await permissionsService.DeletePermissionAsync(id);
            return LpApiResponse.Create();
        }
        #endregion
    }
}
