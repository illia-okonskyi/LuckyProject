using LuckyProject.AuthServer.Services.LpApi.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace LuckyProject.AuthServer.Services.LpApi
{
    public interface ILpApiService
    {
        Task InstallAsync(string callbackUrl, CancellationToken cancellationToken = default);
        Task UninstallAsync(Guid id);
        Task<List<ApiResponse>> GetAllApisAsync(CancellationToken cancellationToken = default);
        Task<ApiResponse> GetApiByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<UserFeaturesResponse> GetUserFeaturesAsync(
            Guid userId,
            CancellationToken cancellationToken = default);
    }
}
