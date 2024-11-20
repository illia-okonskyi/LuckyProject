using LuckyProject.AuthServer.Services.Permissions.Requests;
using LuckyProject.AuthServer.Services.Permissions.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.Permissions
{
    public interface IPermissionsService
    {
        #region CRUD
        Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request);
        Task<PaginatedList<PermissionResponse>> GetPermissionsAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<PermissionResponse> GetPermissionByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<PermissionResponse> GetPermissionByFullNameAsync(
            string fullName,
            CancellationToken cancellationToken = default);
        Task<PermissionResponse> UpdatePermissionAsync(UpdatePermissionRequest request);
        Task DeletePermissionAsync(Guid id, bool ignoreSealed = false);
        #endregion
    }
}
