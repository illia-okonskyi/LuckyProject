using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.LpApi;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Web.Attributes;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Controllers.Api.Self
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [LpJwtAuthorize("rSA")]
    public class ApiController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private class AvailableLpApi
        {
            public string Name { get; init; }
            public string CallbackUrl { get; init; }
        }
        private static readonly List<AvailableLpApi> AvailableLpApis = [
            new()
            {
                Name = "Admin-Panel",
                CallbackUrl = "https://localhost:5000/api/admin/callback"
            }];


        private readonly ILpApiService apiService;

        public ApiController(ILpApiService apiService)
        {
            this.apiService = apiService;
        }
        #endregion

        #region Get
        public class ApiPayload
        {
            public Guid Id { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }
        }

        [HttpGet]
        public async Task<LpApiResponse<List<ApiPayload>>> GetAllInstalledApisAsync(
            CancellationToken cancellationToken = default)
        {
            var apis = await apiService.GetAllApisAsync(cancellationToken);
            return LpApiResponse.Create(apis
                .Select(a => new ApiPayload
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description
                })
                .ToList());
        }

        public class LpApiPayload
        {
            public string Name { get; init; }
            public bool IsInstalled { get; init; }
            public Guid? Id { get; init; }
        }

        [HttpGet]
        [Route("lp")]
        public async Task<LpApiResponse<List<LpApiPayload>>> GetAvailableLpApisAsync(
            CancellationToken cancellationToken = default)
        {
            var apis = await apiService.GetAllApisAsync(cancellationToken);
            return LpApiResponse.Create(AvailableLpApis
                .Select(a =>
                {
                    var api = apis.FirstOrDefault(api => api.Name == a.Name);
                    return new LpApiPayload
                    {
                        Name = a.Name,
                        IsInstalled = api != null,
                        Id = api?.Id
                    };
                })
                .ToList());
        }
        #endregion

        #region Install
        [HttpPost]
        public async Task<LpApiResponse> InstallApiAsync(
            [FromBody] string callbackUrl,
            CancellationToken cancellationToken = default)
        {
            await apiService.InstallAsync(callbackUrl, cancellationToken);
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("lp")]
        public async Task<LpApiResponse> InstallLpApiAsync(
            [FromBody] string apiName,
            CancellationToken cancellationToken = default)
        {
            var callbackUrl = AvailableLpApis.FirstOrDefault(a => a.Name == apiName)?.CallbackUrl;
            if (string.IsNullOrEmpty(callbackUrl))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(apiName),
                    ValidationErrorCodes.NotFound);
            }

            return await InstallApiAsync(callbackUrl, cancellationToken);
        }
        #endregion

        #region Uninstall
        [HttpDelete]
        public async Task<LpApiResponse> UninstallApiAsync(
            [FromBody] Guid id,
            CancellationToken cancellationToken = default)
        {
            await apiService.UninstallAsync(id);
            return LpApiResponse.Create();
        }
        #endregion
    }
}
