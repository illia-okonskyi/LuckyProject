using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using LuckyProject.Lib.Web.Models.AuthorizationAuthentication;
using LuckyProject.Lib.Web.Models.Requests;
using LuckyProject.Lib.Web.Services;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Middleware
{
    public class DefaultJwtAuthorizationAuthenticationHandler
         : ILpJwtAuthorizationAuthenticationHandler
    {
        private readonly DefaultJwtAuthorizationAuthenticationHandlerOptions options;
        private readonly ILpHttpService httpService;
        private readonly HttpClient httpClient;

        public DefaultJwtAuthorizationAuthenticationHandler(
            IOptions<DefaultJwtAuthorizationAuthenticationHandlerOptions> options,
            ILpHttpService httpService)
        {
            this.options = options.Value;
            this.httpService = httpService;
            httpClient = httpService.CreateClient(
                LpWebConstants.Internals.HttpClients.DefaultJwtAuthorizationAuthenticationHandler);
        }

        public async Task<LpIdentity> HasAccessAsync(
            string origin,
            string token,
            List<LpAuthRequirement> requirements)
        {
            var response = await httpService.LpApiRequestAsync<LpAuthResponse>(
                httpClient,
                new()
                {
                    Method = HttpMethod.Post,
                    BaseUrl = $"{options.AuthServerEndpoint}/api/LpApiAuth",
                    Content = new LpAuthRequest
                    {
                        Origin = origin,
                        Token = token,
                        Requirements = requirements
                    }
                });
            return response.Identity;
        }
    }
}
