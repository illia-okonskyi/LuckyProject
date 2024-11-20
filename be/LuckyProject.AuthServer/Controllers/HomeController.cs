using LuckyProject.Lib.Web.Constants;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LuckyProject.AuthServer.Controllers
{
    [Route("/")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View(model: DateTime.UtcNow.ToString("u"));
    }
}
