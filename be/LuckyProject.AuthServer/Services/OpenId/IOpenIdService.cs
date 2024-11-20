using LuckyProject.AuthServer.Services.OpenId.Requests;
using LuckyProject.AuthServer.Services.OpenId.Responses;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.OpenId
{
    public interface IOpenIdService
    {
        #region Helpers
        string GetClientTypePrefix(LpApiClientType type);
        LpApiClientType GetClientType(string clientId);
        string GetClientId(LpApiClientType type, string name);
        string GetClientName(string clientId);
        string GenerateMachineClientSecret();
        #endregion

        #region Clients CRUD
        Task<ClientResponse> CreateWebClientAsync(
            CreateWebClientRequest request,
            CancellationToken cancellationToken = default);
        Task<ClientResponse> CreateMachineClientAsync(
             CreateMachineClientRequest request,
             CancellationToken cancellationToken = default);
        Task<PaginatedList<ClientResponse>> GetClientsAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task<ClientResponse> GetClientByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);
        Task<ClientResponse> GetClientByClientIdAsync(
            string clientId,
            CancellationToken cancellationToken = default);
        Task<ClientResponse> GetClientByTypeAndNameAsync(
            LpApiClientType type,
            string name,
            CancellationToken cancellationToken = default);
        Task UpdateMachineClientSecretAsync(
            Guid id,
            string secret,
            CancellationToken cancellationToken = default);
        Task DeleteClientAsync(
            Guid id,
            bool ignoreSealed = false,
            CancellationToken cancellationToken = default);
        #endregion

        #region User Authorizations
        Task<UserAuthorizationResponse> CreateUserAuthorizationAsync(
            Guid clientId,
            Guid userId,
            ClaimsIdentity identity,
            CancellationToken cancellationToken = default);
        Task<List<UserAuthorizationResponse>> GetUserAuthorizationsAsync(
            Guid clientId,
            Guid userId,
            CancellationToken cancellationToken = default);
        Task RevokeUserAuthorizationsAsync(
             Guid clientId,
             Guid userId,
             CancellationToken cancellationToken = default);
        #endregion

        #region Tokens
        Task<(bool Success, LpIdentity Identity)> ValidateAccessTokenAsync(
            string token,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
