using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace LuckyProject.Lib.Web.Controllers
{
    public class LpApiControllerBase : ControllerBase
    {
        protected IActionResult LpApiResponseResult()
        {
            return new JsonResult(LpApiResponse.Create());
        }

        protected IActionResult LpApiResponseResult(AbstractLpApiError lpApiError)
        {
            return new JsonResult(LpApiResponse.Create(lpApiError));
        }

        protected IActionResult LpApiResponseResult<TPayload>(
            TPayload payload,
            object serializerSettings = null)
        {
            return new JsonResult(LpApiResponse.Create(payload), serializerSettings);
        }
    }
}
