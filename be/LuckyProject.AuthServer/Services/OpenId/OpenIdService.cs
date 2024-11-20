using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Services.Cors;
using LuckyProject.AuthServer.Services.OpenId.Constants;
using LuckyProject.AuthServer.Services.OpenId.Models;
using LuckyProject.AuthServer.Services.OpenId.Requests;
using LuckyProject.AuthServer.Services.OpenId.Responses;
using LuckyProject.AuthServer.Services.OpenId.Validators;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Web.Extensions;
using LuckyProject.Lib.Web.Helpers;
using LuckyProject.Lib.Web.Models;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.OpenId
{
    public class OpenIdService : IOpenIdService
    {
        #region Internals & ctor & Dispose
        private class CustomProperties
        {
            public bool IsSealed { get; init; }

            public JsonElement ToJsonElement()
            {
                return JsonSerializer.SerializeToElement(this);
            }

            public class StoredProperties
            {
                public CustomProperties LpCustom { get; init; }
            }

            public static CustomProperties FromString(string value)
            {
                return JsonSerializer.Deserialize<StoredProperties>(value).LpCustom;
            }
        }

        private readonly AuthServerDbContext dbContext;
        private readonly IOpenIddictApplicationManager appManager;
        private readonly IOpenIddictAuthorizationManager authManager;
        private readonly IStringService stringService;
        private readonly IAuthServerCorsService corsService;
        private readonly IValidationService validationService;
        private readonly IJsonService jsonService;
        private readonly IUsersService usersService;
        private readonly IAuthSessionManager authSessionManager;
        private readonly OpenIddictValidationService openIddictValidationService;

        public OpenIdService(
            AuthServerDbContext dbContext,
            IOpenIddictApplicationManager appManager,
            IOpenIddictAuthorizationManager authManager,
            IStringService stringService,
            IAuthServerCorsService corsService,
            IValidationService validationService,
            IJsonService jsonService,
            IUsersService usersService,
            IAuthSessionManager authSessionManager,
            OpenIddictValidationService openIddictValidationService)
        {
            this.dbContext = dbContext;
            this.appManager = appManager;
            this.authManager = authManager;
            this.stringService = stringService;
            this.corsService = corsService;
            this.validationService = validationService;
            this.jsonService = jsonService;
            this.usersService = usersService;
            this.authSessionManager = authSessionManager;
            this.openIddictValidationService = openIddictValidationService;
        }
        #endregion

        #region Helpers
        public string GetClientTypePrefix(LpApiClientType type) =>
            LpApiClientTypeHelper.GetClientTypePrefix(type);
        public LpApiClientType GetClientType(string clientId) =>
            LpApiClientTypeHelper.GetClientType(clientId); 
        public string GetClientId(LpApiClientType type, string name) =>
            LpApiClientTypeHelper.GetClientId(type, name);
        public string GetClientName(string clientId) =>
            LpApiClientTypeHelper.GetClientName(clientId);

        public string GenerateMachineClientSecret()
        {
            return stringService.GenerateRandomString(
                ServiceConstants.MachineClientSecretChars,
                64);
        }
        #endregion

        #region Clients CRUD
        public async Task<ClientResponse> CreateWebClientAsync(
            CreateWebClientRequest request,
            CancellationToken cancellationToken = default)
        {
            request.ClientId = GetClientId(LpApiClientType.Web, request.Name);
            var allClientIds = await GetAllClientIdsAsync();
            validationService.EnsureValid(
                request,
                new CreateWebClientRequestValidator(allClientIds));

            var desc = new OpenIddictApplicationDescriptor
            {
                ClientId = request.ClientId,
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                DisplayName = request.DisplayName,
                ClientType = OpenIddictConstants.ClientTypes.Public,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Profile
                },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                },
                Properties =
                {
                    {
                        nameof(CustomProperties.StoredProperties.LpCustom),
                        (new CustomProperties { IsSealed = request.IsSealed }).ToJsonElement()
                    }
                }
            };
            foreach (var o in request.Origins)
            {
                desc.RedirectUris.Add(o.RedirectUrl);
                if (o.PostLogoutRedirectUrl != null)
                {
                    desc.PostLogoutRedirectUris.Add(o.PostLogoutRedirectUrl);
                }
            }

            var app = await CreateClientAsync(
                desc,
                request.Origins.Select(o => o.BaseUrl).ToHashSet(),
                cancellationToken);
            var props = CustomProperties.FromString(app.Properties);
            return new ClientResponse
            {
                Id = Guid.Parse(app.Id),
                Type = LpApiClientType.Web,
                ClientId = app.ClientId,
                Name = request.Name,
                DisplayName = app.DisplayName,
                IsSealed = props.IsSealed,
                Web = new()
                {
                    Origins = request.Origins
                }
            };
        }

        public async Task<ClientResponse> CreateMachineClientAsync(
             CreateMachineClientRequest request,
             CancellationToken cancellationToken = default)
        {
            request.ClientId = GetClientId(LpApiClientType.Machine, request.Name);
            var allClientIds = await GetAllClientIdsAsync();
            validationService.EnsureValid(
                request,
                new CreateMachineClientRequestValidator(allClientIds));

            var desc = new OpenIddictApplicationDescriptor
            {
                ClientId = request.ClientId,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                DisplayName = $"{request.Name} Machine Client",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ClientSecret = request.Secret,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                },
                Properties =
                {
                    {
                        nameof(CustomProperties.StoredProperties.LpCustom),
                        (new CustomProperties { IsSealed = request.IsSealed }).ToJsonElement()
                    }
                }
            };

            using var transaction = await dbContext
                .Database
                .BeginTransactionIfNoCurrentAsync(cancellationToken);
            try
            {
                var app = await CreateClientAsync(desc, request.Origins, cancellationToken);
                var user = await usersService.CreateMachineUserAsync(new()
                {
                    ClientName = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    PreferredLocale = request.PreferredLocale,
                    MachineClientId = app.Id
                });
                await transaction.CommitOrSkipIfNullAsync(cancellationToken);
                var props = CustomProperties.FromString(app.Properties);
                return new ClientResponse
                {
                    Id = Guid.Parse(app.Id),
                    Type = LpApiClientType.Machine,
                    Name = request.Name,
                    ClientId = app.ClientId,
                    DisplayName = app.DisplayName,
                    IsSealed = request.IsSealed,
                    Machine = new()
                    {
                        UserId = user.Id,
                        Origins = request.Origins
                    }
                };
            }
            catch (Exception)
            {
                await transaction.RollbackOrSkipIfNullAsync();
                throw;
            }
        }

        public async Task<PaginatedList<ClientResponse>> GetClientsAsync(
            LpFilterOrderPaginationRequest request,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var clients = await appManager.ListAsync(apps =>
            {
                var q = apps.Select(a => (OpenIddictEntityFrameworkCoreApplication)a);

                var clientTypeFilter = request.GetEnumFilter<LpApiClientType>("type");
                if (clientTypeFilter.HasValue)
                {
                    var prefix = GetClientTypePrefix(clientTypeFilter.Value);
                    q = q.Where(a => a.ClientId.Substring(0, 4) == prefix);
                }
                var nameFilter = request.GetFilter<string>("name");
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    q = q.Where(a => a.ClientId.Substring(4).Contains(nameFilter));
                }

                if (request.Order == "id-asc")
                {
                    q = q.OrderBy(a => a.Id);
                }
                else if (request.Order == "id-desc")
                {
                    q = q.OrderByDescending(a => a.Id);
                }
                else if (request.Order == "type-asc")
                {
                    q = q.OrderBy(a => a.ClientId.Substring(0, 4));
                }
                else if (request.Order == "type-desc")
                {
                    q = q.OrderByDescending(a => a.ClientId.Substring(0, 4));
                }
                else if (request.Order == "name-asc")
                {
                    q = q.OrderBy(a => a.ClientId.Substring(4));
                }
                else if (request.Order == "name-desc")
                {
                    q = q.OrderByDescending(a => a.ClientId.Substring(4));
                }
                else
                {
                    q = q.OrderBy(a => a.Id);
                }

                return q;
            },
            cancellationToken).ToListAsync(cancellationToken);
            var paginatedClients = clients.ToPaginatedList(pageSize, request.Page);
            var resultItems = new List<ClientResponse>();
            foreach (var client in paginatedClients.Items)
            {
                resultItems.Add(await ToClientResponseAsync(client, cancellationToken));
            }

            return resultItems.ToPaginatedList(paginatedClients.Pagination);
        }


        public async Task<ClientResponse> GetClientByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var app = await appManager.FindByIdAsync(id.ToString(), cancellationToken);
            if (app == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            return await ToClientResponseAsync(app, cancellationToken);
        }

        public async Task<ClientResponse> GetClientByClientIdAsync(
            string clientId,
            CancellationToken cancellationToken = default)
        {
            var app = await appManager.FindByClientIdAsync(clientId, cancellationToken);
            if (app == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(clientId),
                    ValidationErrorCodes.NotFound);
            }

            return await ToClientResponseAsync(app, cancellationToken);
        }

        public async Task<ClientResponse> GetClientByTypeAndNameAsync(
            LpApiClientType type,
            string name,
            CancellationToken cancellationToken = default)
        {
            return await GetClientByClientIdAsync(GetClientId(type, name), cancellationToken);
        }

        public async Task UpdateMachineClientSecretAsync(
            Guid id,
            string secret,
            CancellationToken cancellationToken = default)
        {
            if (!ServiceConstants.MachineClientSecretRegex.IsMatch(secret))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(secret),
                    ValidationErrorCodes.Regex,
                    new()
                    {
                        {
                            ValidationErrorCodes.Details.Target,
                            ServiceConstants.MachineClientSecretRegex.ToString()
                        }
                    });
            }

            var app = await appManager.FindByIdAsync(id.ToString(), cancellationToken);
            if (app == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            var clientId = ((OpenIddictEntityFrameworkCoreApplication)app).ClientId;
            if (GetClientType(clientId) != LpApiClientType.Machine)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            await appManager.UpdateAsync(app, secret, cancellationToken);
        }

        public async Task DeleteClientAsync(
            Guid id,
            bool ignoreSealed = false,
            CancellationToken cancellationToken = default)
        {
            var app = (OpenIddictEntityFrameworkCoreApplication)
                (await appManager.FindByIdAsync(id.ToString(), cancellationToken));
            if (app == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            var props = CustomProperties.FromString(app.Properties);
            if (!ignoreSealed && props.IsSealed)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.AccessDenied);
            }

            await appManager.DeleteAsync(app, cancellationToken);
        }
        #endregion

        #region User Authorizations
        public async Task<UserAuthorizationResponse> CreateUserAuthorizationAsync(
            Guid clientId,
            Guid userId,
            ClaimsIdentity identity,
            CancellationToken cancellationToken = default)
        {
            var authorization = await authManager.CreateAsync(
                client: clientId.ToString(),
                subject: userId.ToString(),
                identity: identity,
                type: OpenIddictConstants.AuthorizationTypes.AdHoc,
                scopes: identity.GetScopes(),
                cancellationToken: cancellationToken);
            return ToUserAuthorizationResponse(authorization);
        }

        public async Task<List<UserAuthorizationResponse>> GetUserAuthorizationsAsync(
            Guid clientId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var authorizations = await authManager.FindAsync(
                client: clientId.ToString(),
                subject: userId.ToString(),
                status: OpenIddictConstants.Statuses.Valid)
                .ToListAsync(cancellationToken);
            return authorizations.Select(ToUserAuthorizationResponse).ToList();
        }

        public async Task RevokeUserAuthorizationsAsync(
            Guid clientId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var authorizations = await authManager.FindAsync(
                client: clientId.ToString(),
                subject: userId.ToString(),
                status: OpenIddictConstants.Statuses.Valid)
                .ToListAsync(cancellationToken);
            foreach (var authorization in authorizations)
            {
                await authManager.TryRevokeAsync(authorization, cancellationToken);
            }
        }
        #endregion

        #region Internals
        private async Task<HashSet<string>> GetAllClientIdsAsync()
        {
            return (await dbContext.OpenIddictApplications.Select(a => a.ClientId).ToListAsync())
                .ToHashSet();
        }

        private async Task<OpenIddictEntityFrameworkCoreApplication> CreateClientAsync(
            OpenIddictApplicationDescriptor desc,
            HashSet<Uri> origins,
            CancellationToken cancellationToken)
        {
            using var transaction = await dbContext
                .Database
                .BeginTransactionIfNoCurrentAsync(cancellationToken);
            try
            {
                var result = (OpenIddictEntityFrameworkCoreApplication)
                    await appManager.CreateAsync(desc, cancellationToken);
                await corsService.CreateConfigAsync(dbContext, new()
                {
                    ClientId = desc.ClientId,
                    Origins = origins
                });
                await transaction.CommitOrSkipIfNullAsync(cancellationToken);
                return result;
            }
            catch (Exception)
            {
                await transaction.RollbackOrSkipIfNullAsync();
                throw;
            }
        }

        private async Task<ClientResponse> ToClientResponseAsync(
            object appObject,
            CancellationToken cancellationToken)
        {
            var app = (OpenIddictEntityFrameworkCoreApplication)appObject;
            var type = GetClientType(app.ClientId);
            var name = GetClientName(app.ClientId);
            var origins = await corsService.GetClientOriginsAsync(
                dbContext,
                app.ClientId,
                cancellationToken);
            var props = CustomProperties.FromString(app.Properties);
            if (type == LpApiClientType.Machine)
            {
                return new ClientResponse
                {
                    Id = Guid.Parse(app.Id),
                    Type = type,
                    Name = name,
                    ClientId = app.ClientId,
                    DisplayName = app.DisplayName,
                    IsSealed = props.IsSealed,
                    Machine = new()
                    {
                        UserId = await usersService.GetMachineClientUserId(
                            app.Id,
                            cancellationToken),
                        Origins = origins.ToHashSet(),
                    }
                };
            }

            // NOTE: Web client type
            var redirectUrls = jsonService
                .DeserializeObject<List<string>>(app.RedirectUris)
                .Select(s => new Uri(s, UriKind.Absolute))
                .ToList();
            var postLogoutredirectUrls = new List<Uri>();
            if (!string.IsNullOrEmpty(app.PostLogoutRedirectUris))
            {
                postLogoutredirectUrls = jsonService
                    .DeserializeObject<List<string>>(app.PostLogoutRedirectUris)
                    .Select(s => new Uri(s, UriKind.Absolute))
                    .ToList();
            }
                
            return new ClientResponse
            {
                Id = Guid.Parse(app.Id),
                Type = type,
                Name = name,
                ClientId = app.ClientId,
                DisplayName = app.DisplayName,
                IsSealed = props.IsSealed,
                Web = new()
                {
                    Origins = ToClientResponseAsync_CreateWebOrigins(
                        origins,
                        redirectUrls,
                        postLogoutredirectUrls)
                }
            };
        }

        private HashSet<WebClientOrigin> ToClientResponseAsync_CreateWebOrigins(
            List<Uri> origins,
            List<Uri> redirectUrls,
            List<Uri> postLogoutredirectUrls)
        {
            var result = new HashSet<WebClientOrigin>();
            foreach (var o in origins)
            {
                var ru = redirectUrls.First(o.IsBaseOf);
                var plru = postLogoutredirectUrls.FirstOrDefault(o.IsBaseOf);
                result.Add(new(o, ru, plru));
            }

            return result;
        }

        private UserAuthorizationResponse ToUserAuthorizationResponse(object authorizationObject)
        {
            var authorization = (OpenIddictEntityFrameworkCoreAuthorization)authorizationObject;
            return new UserAuthorizationResponse
            {
                Id = Guid.Parse(authorization.Id),
            };
        }
        #endregion

        #region Tokens
        public async Task<(bool Success, LpIdentity Identity)> ValidateAccessTokenAsync(
            string token,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var principal = await openIddictValidationService.ValidateAccessTokenAsync(
                    token,
                    cancellationToken);
                var lpIdentity = principal.GetLpIdentity();
                if (lpIdentity == null)
                {
                    return (false, null);
                }

                if (!await authSessionManager.IsSessionAliveAndValidAsync(
                    lpIdentity.SessionId,
                    cancellationToken))
                {
                    return (false, null);
                }

                return (true, lpIdentity);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }
        #endregion
    }
}
