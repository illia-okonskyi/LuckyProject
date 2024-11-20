using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Controllers;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace LuckyProject.AuthServer.Controllers.Api.Public
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class VersionController : LpApiControllerBase
    {
        private readonly IAppVersionService appVersionService;

        public VersionController(IAppVersionService appVersionService)
        {
            this.appVersionService = appVersionService;
        }

        [HttpGet]
        public LpApiResponse<string> GetAppVersion()
        {
            return LpApiResponse.Create(appVersionService.AppVersion);
        }
    }
}
