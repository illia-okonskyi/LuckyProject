using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Constants;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Middleware
{
    public class LpDynamicCorsPolicyProvider : ICorsPolicyProvider
    {
        #region Internals & ctor
        private static readonly CorsPolicy DisallowAllPolicy =
            new CorsPolicyBuilder(new CorsPolicy()).Build();
        private static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler =
            new JwtSecurityTokenHandler();

        private readonly LpDynamicCorsPolicyProviderOptions options;
        private readonly DefaultCorsPolicyProvider fallback;
        private readonly IStringService stringService;
        private readonly ILpDynamicCorsPolicyResolver policyResolver;
        private readonly ILogger logger;

        public LpDynamicCorsPolicyProvider(
            IOptions<LpDynamicCorsPolicyProviderOptions> options,
            DefaultCorsPolicyProvider fallback,
            IStringService stringService,
            ILpDynamicCorsPolicyResolver policyResolver,
            ILogger<LpDynamicCorsPolicyProvider> logger)
        {
            this.options = options.Value;
            this.fallback = fallback;
            this.stringService = stringService;
            this.policyResolver = policyResolver;
            this.logger = logger;
        }
        #endregion

        #region Interface impl
        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            var forwardToFallback = policyName != LpWebConstants.Cors.DynamicPolicyName;
            if (forwardToFallback)
            {
                return await fallback.GetPolicyAsync(context, policyName);
            }

            string origin = context.Request.Headers.Origin.ToString();
            if (!string.IsNullOrEmpty(origin))
            {
                origin = stringService.GetUriSchemeAndAuthority(new Uri(origin), false);
            }

            if (options.SelfOrigins.Contains(origin))
            {
                return new CorsPolicyBuilder(origin)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .Build();
            }

            var policy = await ResolvePolicyAsync(context, origin);
            return policy ?? DisallowAllPolicy;
        }
        #endregion

        #region Internals
        private async Task<CorsPolicy> ResolvePolicyAsync(HttpContext httpContext, string origin)
        {
            try
            {
                return await policyResolver.ResolvePolicyAsync(httpContext, origin);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to resolve Dynamic CORS policy");
                return null;
            }
        }
        #endregion
    }
}
