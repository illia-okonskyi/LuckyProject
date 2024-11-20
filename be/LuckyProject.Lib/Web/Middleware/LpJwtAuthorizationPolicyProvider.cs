using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Middleware
{
    public class LpJwtAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly IAuthorizationPolicyProvider fallback;

        public LpJwtAuthorizationPolicyProvider(
            IOptions<AuthorizationOptions> options)
        {
            fallback = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
            fallback.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
            fallback.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (!policyName.StartsWith(
                LpWebConstants.Auth.Jwt.AuthoriaztionPolicyPrefix,
                StringComparison.Ordinal))
            {
                return await fallback.GetPolicyAsync(policyName);
            }

            var policy = new AuthorizationPolicyBuilder(
                LpWebConstants.Auth.Jwt.AuthenticationScheme);
            policy.AddRequirements(new LpJwtAuthorizationRequirement
            {
                Requirements = LpJwtHelper.ParseAuthRequirements(
                    policyName[LpWebConstants.Auth.Jwt.AuthoriaztionPolicyPrefix.Length..])
            });
            return policy.Build();
        }
    }

}
