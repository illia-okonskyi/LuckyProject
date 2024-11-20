using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.AuthServer.Services.Users.Responses;
using LuckyProject.AuthServer.Services.Roles.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Users
{
    public interface IUsersService
    {
        #region Helpers
        string GetMachineUserName(string clientName);
        #endregion

        #region CRUD
        Task<UserResponse> CreateWebUserAsync(CreateWebUserRequest request);
        Task<UserResponse> CreateMachineUserAsync(CreateMachineUserRequest request);
        Task<PaginatedList<UserResponse>> GetUsersAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserByUserNameAsync(
             string userName,
             CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserByEmailAsync(
             string email,
             CancellationToken cancellationToken = default);
        Task<Guid> GetMachineClientUserId(
            string machineClientId,
            CancellationToken cancellationToken = default);
        Task<UserResponse> UpdateWebUserAsync(UpdateWebUserRequest request);
        Task<UserResponse> UpdateMachineUserAsync(UpdateMachineUserRequest request);
        Task DeleteUserAsync(Guid id, bool ignoreSealed = false, bool ignoreMachine = false);
        #endregion

        #region User-Roles management
        Task<List<Guid>> GetUserRoleIdsAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<RoleResponse>> GetUserRolesAsync(
            Guid userId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task AssignRoleToUserAsync(UserRoleRequest request);
        Task DeleteRoleFromUserAsync(UserRoleRequest request);
        #endregion

        #region User-permissions management
        Task<PaginatedList<UserPermissionsResponse>> GetUserPermissionsAsync(
            Guid userId,
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<AuthorizeResponse> AuthorizeAsync(
            List<LpAuthRequirement> requirements,
            Guid userId,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
