using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Middleware
{
    public interface ILpDynamicCorsPolicyResolver
    {
        Task<CorsPolicy> ResolvePolicyAsync(HttpContext httpContext, string origin);
    }
}
