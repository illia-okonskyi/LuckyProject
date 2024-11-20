using LuckyProject.AuthServer.Models;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Controllers;
using LuckyProject.Lib.Web.Models.Callback;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LuckyProject.AuthServer.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class CallbackController : LpApiCallbackControllerBase
    {
        #region Internals & ctor
        private readonly WebServerOptions webServerOptions;

        public CallbackController(
            IOptions<WebServerOptions> webServerOptions,
            ILogger<CallbackController> logger)
        {
            this.webServerOptions = webServerOptions.Value;
        }
        #endregion

        #region LpApiCallbackControllerBase
        protected override Task<LpApiInstallResponsePayload> InstallAsync(
            CancellationToken cancellationToken)
        {
            return Task.FromResult(new LpApiInstallResponsePayload()
            {
                Name = "Admin-Panel",
                Description = "Lucky Project Admin Panel API",
                Origins = new() { webServerOptions.Endpoint },
                Endpoint = $"{webServerOptions.Endpoint}/api/admin",
                MachineUserEmail = "illia.okonskyi.work.api.admin@gmail.com",
                MachineUserPhoneNumber = "+380500125351",
                MachineUserPrefferedLocale = "en-US",
                Permissions = new()
                {
                    new()
                    {
                        Type = LpAuthPermissionType.Binary,
                        Name = "Read",
                        Description = "Read Admin Panel Data",
                        Allow = true
                    },
                    new()
                    {
                        Type = LpAuthPermissionType.Binary,
                        Name = "Manage",
                        Description = "Manage Admin Panel Data",
                        Allow = true
                    }
                }
            });
        }

        protected override Task InstalledAsync(
            LpApiInstalledRequest request,
            CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task UninstalledAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected override Task<List<string>> FeaturesAsync(
            LpApiFeaturesRequest request,
            CancellationToken cancellationToken)
        {
            if (request.HasRootPermission())
            {
                return Task.FromResult(new List<string>() { "Read", "Manage" });
            }

            var features = new List<string>();
            if (request.HasBinaryPermission("Read"))
            {
                features.Add("Read");
            }

            if (request.HasBinaryPermission("Manage"))
            {
                features.Add("Manage");
            }

            return Task.FromResult(features);
        }
        #endregion
    }
}
