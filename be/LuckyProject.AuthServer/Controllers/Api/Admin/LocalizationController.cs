using LuckyProject.AuthServer.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Controllers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    public class LocalizationController : LpLocalizationControllerBase
    {
        public LocalizationController(
            IOptions<LocalizationOptions> options,
            ILpLocalizationService localizationService,
            IFsService fsService,
            IJsonService jsonService)
            : base(options.Value.I18nDir, localizationService, fsService, jsonService)
        { }

        [HttpGet]
        [Route("{lng}/{ns}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ContentResult> GetFeTranslationAsync(
            string lng,
            string ns,
            CancellationToken cancellationToken = default)
        {
            try
            {
                ns = ns switch
                {
                    "lp-ui-admin-panel" => "lp-api-admin-panel-fe-ui",
                    _ => throw new NotImplementedException()
                };

                string content = null;
                try
                {
                    content = await BuildTranslationFileContentAsync(
                    lng,
                    ns,
                    cancellationToken);
                }
                catch (Exception)
                {
                    content = await BuildTranslationFileContentAsync(
                        "en-US",
                        ns,
                        cancellationToken);
                }
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = MediaTypeNames.Application.Json,
                    Content = content
                };
            }
            catch (Exception)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
        }
    }
}
