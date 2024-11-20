using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Middleware;
using LuckyProject.Lib.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace LuckyProject.Lib.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region AddLpWebCommonServices
        public static IServiceCollection AddLpWebCommonServices(this IServiceCollection services)
        {
            services.AddExceptionHandler<LpExceptionHandler>();
            services.AddProblemDetails();
            services.AddHttpClient();
            services.AddSingleton<ILpHttpService, LpHttpService>();
            return services;
        }
        #endregion

        #region AddLpWebCorsDynamicPolicyServices
        public static IServiceCollection AddLpWebCorsDynamicPolicy<TPolicyResolver>(
            this IServiceCollection services,
            HashSet<string> selfOrigins,
            Action<CorsOptions> setupAction = null)
            where TPolicyResolver : class, ILpDynamicCorsPolicyResolver
        {
            ArgumentNullException.ThrowIfNull(selfOrigins);

            services.AddCors(options =>
            {
                options.AddPolicy(LpWebConstants.Cors.DynamicPolicyName, new CorsPolicy());
                setupAction?.Invoke(options);
            });

            services.AddOptions<LpDynamicCorsPolicyProviderOptions>().Configure(options =>
            {
                options.SelfOrigins = selfOrigins;
            });
            services.AddTransient<DefaultCorsPolicyProvider>();
            services.AddTransient<ICorsPolicyProvider, LpDynamicCorsPolicyProvider>();
            services.AddSingleton<ILpDynamicCorsPolicyResolver, TPolicyResolver>();
            return services;
        }
        #endregion

        #region AddLpWebAuthenticationAuthorization
        public static IServiceCollection AddLpWebAuthenticationAuthorization<THandler>(
            this IServiceCollection services,
            string authServerEndpoint,
            X509Certificate2 authServerCert)
            where THandler : class, ILpJwtAuthorizationAuthenticationHandler
        {
            var builder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = LpWebConstants.Auth.Jwt.AuthenticationScheme;
            }).AddJwtBearer(LpWebConstants.Auth.Jwt.AuthenticationScheme);

            services.AddSingleton<IAuthorizationPolicyProvider, LpJwtAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, LpJwtAuthorizationHandler>();
            services.AddSingleton<ILpJwtAuthorizationAuthenticationHandler, THandler>();

            if (typeof(THandler) == typeof(DefaultJwtAuthorizationAuthenticationHandler))
            {
                services.AddHttpClient(
                    LpWebConstants.Internals.HttpClients.DefaultJwtAuthorizationAuthenticationHandler);
                services.AddOptions<DefaultJwtAuthorizationAuthenticationHandlerOptions>()
                    .Configure(options =>
                    {
                        options.AuthServerEndpoint = authServerEndpoint;
                    });
            }

            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddLpWebAuthenticationAuthorization(
             this IServiceCollection services,
             string authServerEndpoint,
             X509Certificate2 authServerCert)
        {
            return services
                .AddLpWebAuthenticationAuthorization<DefaultJwtAuthorizationAuthenticationHandler>(
                    authServerEndpoint,
                    authServerCert);
        }
        #endregion
    }
}
