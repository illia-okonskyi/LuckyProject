using Microsoft.AspNetCore.Builder;
using OpenIddict.Abstractions;

namespace LuckyProject.Lib.Swagger.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLpSwagger(
            this IApplicationBuilder app,
            LpSwaggerOptions options)
        {
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint(
                    $"{options.Version}/swagger.json",
                    $"{options.Title}/{options.Version}");
                o.DocumentTitle = $"{options.Title} Swagger UI";

                o.OAuthClientId(options.SwaggerClientId);
                o.OAuthAppName($"{options.SwaggerClientId} Client");
                o.OAuthScopes(
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess);
                o.OAuthUsePkce();
            });
            return app;
        }
    }
}
