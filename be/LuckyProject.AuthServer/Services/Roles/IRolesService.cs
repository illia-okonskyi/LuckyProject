using LuckyProject.AuthServer.Services.Roles.Requests;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.Roles
{
    public interface IRolesService
    {
        #region CRUD
        Task<RoleResponse> CreateRoleAsync(CreateUpdateRoleRequest request);
        Task<PaginatedList<RoleResponse>> GetRolesAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<RoleResponse> GetRoleByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<RoleResponse> GetRoleByNameAsync(
            string name,
            CancellationToken cancellationToken = default);
        Task<RoleResponse> UpdateRoleAsync(CreateUpdateRoleRequest request);
        Task DeleteRoleAsync(Guid id, bool ignoreSealed = false);
        #endregion

        #region Role-Users management
        Task<PaginatedList<UserResponse>> GetRoleUsersAsync(
            Guid roleId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task AssignUserToRoleAsync(RoleUserRequest request);
        Task DeleteUserFromRoleAsync(RoleUserRequest request);
        #endregion

        #region Role-Permissions management
        Task<PaginatedList<RolePermissionsResponse>> GetRolePermissionsAsync(
            Guid roleId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task AssignPermissionToRoleAsync(RolePermissionRequest request);
        Task DeletePermissionFromRoleAsync(RolePermissionRequest request);
        #endregion
    }
}
