using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Web.Models.Callback;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Controllers
{
    public abstract class LpApiCallbackControllerBase : LpApiControllerBase
    {
        [HttpGet]
        [Route("install")]
        public async Task<LpApiResponse<LpApiInstallResponsePayload>> InstallAsyncAction(
            CancellationToken cancellationToken = default)
        {
            return LpApiResponse.Create(await InstallAsync(cancellationToken));
        }

        [HttpPost]
        [Route("install")]
        public async Task<LpApiResponse> InstalledAsyncAction(
            LpApiInstalledRequest request,
            CancellationToken cancellationToken = default)
        {
            await InstalledAsync(request, cancellationToken);
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("uninstall")]
        public async Task<LpApiResponse> UninstalledAsyncAction(
            CancellationToken cancellationToken = default)
        {
            await UninstalledAsync(cancellationToken);
            return LpApiResponse.Create();
        }

        [HttpPost]
        [Route("features")]
        public async Task<LpApiResponse<LpApiFeaturesResponsePayload>> FeaturesAsyncAction(
            LpApiFeaturesRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null || request.Permissions == null || request.Permissions.Count == 0)
            {
                return LpApiResponse.Create(new LpApiFeaturesResponsePayload());
            }

            var features = await FeaturesAsync(request, cancellationToken);
            return LpApiResponse.Create(new LpApiFeaturesResponsePayload() { Features = features });
        }

        protected abstract Task<LpApiInstallResponsePayload> InstallAsync(
            CancellationToken cancellationToken);
        protected abstract Task InstalledAsync(
            LpApiInstalledRequest request,
            CancellationToken cancellationToken);
        protected abstract Task UninstalledAsync(CancellationToken cancellationToken);
        protected abstract Task<List<string>> FeaturesAsync(
            LpApiFeaturesRequest request,
            CancellationToken cancellationToken);
    }
}
