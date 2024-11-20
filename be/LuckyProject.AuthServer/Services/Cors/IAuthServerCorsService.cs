using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Services.Cors.Requests;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Web.Middleware;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.Cors
{
    public interface IAuthServerCorsService : ILpDynamicCorsPolicyResolver
    {
        Task CreateConfigAsync(
            CreateConfigRequest request,
            CancellationToken cancellationToken = default);
        Task CreateConfigAsync(
            AuthServerDbContext dbContext,
            CreateConfigRequest request,
            CancellationToken cancellationToken = default);
        Task<PaginatedList<Uri>> GetClientOriginsAsync(
            string clientId,
            int pageSize,
            int page,
            CancellationToken cancellationToken = default);
        Task<List<Uri>> GetClientOriginsAsync(
            AuthServerDbContext dbContext,
            string clientId,
            CancellationToken cancellationToken = default);
        Task<bool> IsOriginAllowedAsync(
            string origin,
            CancellationToken cancellationToken = default);
    }
}
