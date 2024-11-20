using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Web.Controllers;
using LuckyProject.Lib.Web.Extensions;
using LuckyProject.Lib.Web.Models;

namespace LuckyProject.AuthServer.Controllers
{
    public class AuthServierApiControllerBase : LpApiControllerBase
    {
        protected LpIdentity GetLpIdentity()
        {
            var identity = HttpContext.User.GetLpIdentity();
            if (identity == null)
            {
                throw new LpAccessDeniedAuthException();
            }

            return identity;
        }
    }
}
