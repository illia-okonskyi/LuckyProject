using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.OpenId.Responses;
using LuckyProject.AuthServer.Services.OpenId.Models;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Web.Attributes;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Controllers.Api.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [EnableCors(LpWebConstants.Cors.DynamicPolicyName)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [LpJwtAuthorize("bapi-Admin-Panel-Read")]
    public class ClientsController : AuthServierApiControllerBase
    {
        #region Internals & ctor
        private readonly IOpenIdService openIdService;

        public ClientsController(IOpenIdService openIdService)
        {
            this.openIdService = openIdService;
        }
        #endregion

        #region Create
        public class WebOrigin
        {
            public Uri BaseUrl { get; init; }
            public string Redirect { get; init; }
            public string PostLogoutRedirect { get; init; }
        }

        public class CreateWebRequest
        {
            public string Name { get; init; }
            public string DisplayName { get; init; }
            public List<WebOrigin> Origins { get; init; } = new();
        }

        [HttpPost]
        [Route("web")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<ClientResponse>> CreateWebClientAsync(
            CreateWebRequest request,
            CancellationToken cancellationToken = default)
        {
            var origins = new HashSet<WebClientOrigin>();
            for (var i = 0; i < request.Origins.Count; ++i)
            {
                try
                {
                    var o = request.Origins[i];
                    origins.Add(new WebClientOrigin(o.BaseUrl, o.Redirect, o.PostLogoutRedirect));
                }
                catch
                {
                    AuthServerThrowHelper.ThrowValidationErrorException(
                        $"Origins[{i}]",
                        ValidationErrorCodes.Invalid);
                }
            }

            var client = await openIdService.CreateWebClientAsync(new()
            {
                Name = request.Name,
                DisplayName = request.DisplayName,
                Origins = origins
            });
            return LpApiResponse.Create(client);
        }

        public class CreateMachineRequest
        {
            public string Name { get; init; }
            public string Email { get; init; }
            public string PhoneNumber { get; init; }
            public string PreferredLocale { get; init; }
            public HashSet<Uri> Origins { get; init; }
            public string Secret { get; init; }
        }

        [HttpGet]
        [Route("secret")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public LpApiResponse<string> GenerateMachineClientSecret()
        {
            var secret = openIdService.GenerateMachineClientSecret();
            return LpApiResponse.Create(secret);
        }

        [HttpPost]
        [Route("machine")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse<ClientResponse>> CreateMachineClientAsync(
            CreateMachineRequest request,
            CancellationToken cancellationToken = default)
        {
            var client = await openIdService.CreateMachineClientAsync(new()
            {
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PreferredLocale = request.PreferredLocale,
                Origins = request.Origins,
                Secret = request.Secret
            });
            return LpApiResponse.Create(client);
        }
        #endregion

        #region Get
        [HttpPost]
        [Route("list")]
        public async Task<LpApiResponse<PaginatedList<ClientResponse>>> GetClientsAsync(
            LpFilterOrderPaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var clients = await openIdService.GetClientsAsync(
                request,
                cancellationToken: cancellationToken);
            return LpApiResponse.Create(clients);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<LpApiResponse<ClientResponse>> GetClientAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var client = await openIdService.GetClientByIdAsync(id, cancellationToken);
            return LpApiResponse.Create(client);
        }
        #endregion

        #region Update
        public class UpdateMachineSecretRequest
        {
            public Guid Id { get; init; }
            public string Secret { get; init; }
        }

        [HttpPut]
        [Route("machine-secret")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> UpdateMachineClientSecretAsync(
            UpdateMachineSecretRequest request,
            CancellationToken cancellationToken = default)
        {
            await openIdService.UpdateMachineClientSecretAsync(
                request.Id,
                request.Secret,
                cancellationToken);
            return LpApiResponse.Create();
        }
        #endregion

        #region Delete
        [HttpDelete]
        [Route("{id}")]
        [LpJwtAuthorize("bapi-Admin-Panel-Manage")]
        public async Task<LpApiResponse> DeleteClientAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await openIdService.DeleteClientAsync(id, cancellationToken: cancellationToken);
            return LpApiResponse.Create();
        }
        #endregion
    }
}
