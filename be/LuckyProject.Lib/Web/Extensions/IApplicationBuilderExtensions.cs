using LuckyProject.Lib.Web.Constants;
using Microsoft.AspNetCore.Builder;

namespace LuckyProject.Lib.Web.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLpWebExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler();
        }

        public static IApplicationBuilder UseLpWebCorsDynamicPolicy(this IApplicationBuilder app)
        {
            return app.UseCors(LpWebConstants.Cors.DynamicPolicyName);
        }

        public static IApplicationBuilder UseLpWebAuthenticationAuthorization(
            this IApplicationBuilder app)
        {
            return app.UseAuthentication().UseAuthorization();
        }
    }
}
