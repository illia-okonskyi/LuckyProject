using LuckyProject.Lib.Swagger.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Swagger.Extensions
{
    /// <summary>
    /// At least one client data must must be specified:
    /// - Web Client: WebClientId
    /// - Machine Client: MachineClientId & MachineClientSecret
    /// </summary>
    public class LpSwaggerOptions
    {
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> XmlDocs { get; set; }
        public string AuthServerEndpoint { get; set; }
        public string SwaggerClientId { get; set; }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLpSwagger(
            this IServiceCollection services,
            LpSwaggerOptions options)
        {
            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(options.Version, new OpenApiInfo
                {
                    Version = options.Version,
                    Title = options.Title,
                    Description = options.Description,
                });

                options.XmlDocs.ForEach(d => o.IncludeXmlComments(d));

                o.OperationFilter<LpJwtAuthorizeOperationFilter>();

                var scheme = new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new()
                    {
                        AuthorizationCode = new()
                        {
                            AuthorizationUrl =
                                new($"{options.AuthServerEndpoint}/api/connect/authorize-start"),
                            TokenUrl = new($"{options.AuthServerEndpoint}/api/connect/token"),
                            RefreshUrl = new($"{options.AuthServerEndpoint}/api/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { OpenIddictConstants.Scopes.Profile, "Profile scope" },
                                { OpenIddictConstants.Scopes.OfflineAccess, "Refresh token scope" }
                            }
                        }
                    }
                };
                o.AddSecurityDefinition("lp-auth", scheme);
                o.CustomSchemaIds(s => s.FullName.Replace("+", "."));
            });
            services.AddSwaggerGenNewtonsoftSupport();
            return services;
        }
    }
}
