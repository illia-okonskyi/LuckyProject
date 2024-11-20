using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Helpers;
using LuckyProject.AuthServer.Models;
using LuckyProject.AuthServer.Services.Init;
using LuckyProject.AuthServer.Services.LpApi.Responses;
using LuckyProject.AuthServer.Services.LpApi.Validators;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.Permissions;
using LuckyProject.AuthServer.Services.Roles;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Basics.Validators;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Models.Callback;
using LuckyProject.Lib.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.LpApi
{
    public class LpApiService : ILpApiService
    {
        #region Internals & ctor
        private readonly InitialSeedOptions initialSeedOptions;
        private readonly WebServerOptions webServerOptions;
        private readonly AuthServerDbContext dbContext;
        private readonly ILpHttpService httpService;
        private readonly IPermissionsService permissionsService;
        private readonly IOpenIdService openIdService;
        private readonly IRolesService rolesService;
        private readonly IUsersService usersService;
        private readonly ILpAuthorizationService authService;
        private readonly IValidationService validationService;
        private readonly IAuthSessionManager authSessionManager;

        private readonly HttpClient httpClient;

        public LpApiService(
            IOptions<InitialSeedOptions> initialSeedOptions,
            IOptions<WebServerOptions> webServerOptions,
            AuthServerDbContext dbContext,
            ILpHttpService httpService,
            IPermissionsService permissionsService,
            IOpenIdService openIdService,
            IRolesService rolesService,
            IUsersService usersService,
            ILpAuthorizationService authService,
            IValidationService validationService,
            IAuthSessionManager authSessionManager)
        {
            this.initialSeedOptions = initialSeedOptions.Value;
            this.webServerOptions = webServerOptions.Value;
            this.dbContext = dbContext;
            this.httpService = httpService;
            this.permissionsService = permissionsService;
            this.rolesService = rolesService;
            this.openIdService = openIdService;
            this.usersService = usersService;
            this.authService = authService;
            this.validationService = validationService;
            this.authSessionManager = authSessionManager;

            httpClient = httpService.CreateClient(LpWebConstants.Internals.HttpClients.ApiCallback);
        }
        #endregion

        #region Public interface
        #region InstallAsync
        public async Task InstallAsync(
            string callbackUrl,
            CancellationToken cancellationToken = default)
        {
            if (!Uri.TryCreate(callbackUrl, UriKind.Absolute, out var url))
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(callbackUrl),
                    ValidationErrorCodes.Url,
                    new()
                    {
                        {
                            ValidationErrorCodes.Details.Target,
                            UrlValidatorMode.AbsoluteOnly.ToString()
                        }
                    });
            }

            var apiInstallResponse = await httpService
                .LpApiRequestAsync<LpApiInstallResponsePayload>(
                    httpClient,
                    new()
                    {
                        Method = HttpMethod.Get,
                        BaseUrl = $"{url}/install",
                    },
                    cancellationToken);
            var allApiNames = (await dbContext.Apis
                .AsNoTracking()
                .Select(a => a.Name)
                .ToListAsync(cancellationToken))
                .ToHashSet();
            validationService.EnsureValid(
                apiInstallResponse,
                new LpApiInstallResponsePayloadValidator(allApiNames));

            var machineClientSecret = openIdService.GenerateMachineClientSecret();
            using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var machineClient = await openIdService.CreateMachineClientAsync(new()
                {
                    Name = $"lp-api-{apiInstallResponse.Name}",
                    Origins = apiInstallResponse.Origins.Select(o => new Uri(o)).ToHashSet(),
                    Email = apiInstallResponse.MachineUserEmail,
                    PhoneNumber = apiInstallResponse.MachineUserPhoneNumber,
                    PreferredLocale = apiInstallResponse.MachineUserPrefferedLocale,
                    Secret = machineClientSecret,
                    IsSealed = true
                },
                cancellationToken);
                var authServerUsersRole = await rolesService.GetRoleByNameAsync(
                    initialSeedOptions.AuthServerApi.RoleName,
                    cancellationToken);
                await usersService.AssignRoleToUserAsync(new()
                {
                    UserId = machineClient.Machine.UserId,
                    RoleId = authServerUsersRole.Id
                });

                var apiEntrity = await dbContext.Apis.AddAsync(new()
                {
                    Name = apiInstallResponse.Name,
                    Description = apiInstallResponse.Description,
                    Endpoint = apiInstallResponse.Endpoint,
                    CallbackUrl = callbackUrl,
                    MachineClientId = machineClient.Id.ToString(),
                    MachineUserId = machineClient.Machine.UserId
                },
                cancellationToken);

                var apiId = apiEntrity.Entity.Id;
                foreach (var p in apiInstallResponse.Permissions)
                {
                    await permissionsService.CreatePermissionAsync(new()
                    {
                        Type = p.Type,
                        Name = $"api-{apiInstallResponse.Name}-{p.Name}",
                        Description = p.Description,
                        Allow = p.Allow,
                        Level = p.Level,
                        Passkeys = p.Passkeys,
                        ApiId = apiId,
                        IsSealed = true
                    });
                }
                await dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                await httpService.LpApiRequestAsync(
                    httpClient,
                    new()
                    {
                        Method = HttpMethod.Post,
                        BaseUrl = $"{callbackUrl}/install",
                        Content = new LpApiInstalledRequest
                        {
                            ApiId = apiId,
                            ClientId = machineClient.ClientId,
                            ClientSecret = machineClientSecret,
                            UserId = machineClient.Machine.UserId
                        }
                    });
                await authSessionManager.OnApisChangedAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region UninstallAsync
        public async Task UninstallAsync(Guid id)
        {
            var api = await dbContext.Apis
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new { a.Id, a.CallbackUrl, a.MachineClientId, a.MachineUserId })
                .FirstOrDefaultAsync();
            if (api == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var permissionIds = await dbContext.Permissions
                    .AsNoTracking()
                    .Where(p => p.ApiId == id)
                    .Select(p => p.Id)
                    .ToListAsync();
                dbContext.Apis.Remove(new() { Id = api.Id });
                await openIdService.DeleteClientAsync(
                    Guid.Parse(api.MachineClientId),
                    true);
                await dbContext.RolePermissions
                    .Where(rp => permissionIds.Contains(rp.PermissionId))
                    .ExecuteDeleteAsync();
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                await httpService.LpApiRequestAsync(
                    httpClient,
                    new()
                    {
                        Method = HttpMethod.Post,
                        BaseUrl = $"{api.CallbackUrl}/uninstall"
                    });
                await authSessionManager.OnApisChangedAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion

        #region Getters
        public async Task<List<ApiResponse>> GetAllApisAsync(
            CancellationToken cancellationToken = default)
        {
            return await dbContext.Apis
                .AsNoTracking()
                .Select(a => new ApiResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Endpoint = a.Endpoint,
                    CallbackUrl = a.CallbackUrl,
                    MachineClientId = a.MachineClientId,
                    MachineUserId = a.MachineUserId
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<ApiResponse> GetApiByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var api = await dbContext.Apis
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new ApiResponse
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Endpoint = a.Endpoint,
                    CallbackUrl = a.CallbackUrl,
                    MachineClientId = a.MachineClientId,
                    MachineUserId = a.MachineUserId
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (api == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(id),
                    ValidationErrorCodes.NotFound);
            }

            return api;
        }

        public async Task<UserFeaturesResponse> GetUserFeaturesAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new { u.Id, u.UserName })
                .FirstOrDefaultAsync(cancellationToken);
            if (user == null)
            {
                AuthServerThrowHelper.ThrowValidationErrorException(
                    nameof(userId),
                    ValidationErrorCodes.NotFound);
            }

            var userRoleIds = await dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .OrderBy(ur => ur.RoleId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);
            var userPermissionIds = await dbContext.RolePermissions
                .AsNoTracking()
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);
            var apis = await dbContext.Apis
                .AsNoTracking()
                .Select(a => new { a.Id, a.Name, a.Endpoint, a.CallbackUrl })
                .ToListAsync(cancellationToken);

            var saPermissionName = authService.GetPermissionFullName(
                LpAuthPermissionType.Root,
                initialSeedOptions.Sa.PermissionName);
            var saPermission = await dbContext.Permissions
                .AsNoTracking()
                .Where(p => userPermissionIds.Contains(p.Id) && p.FullName == saPermissionName)
                .FirstOrDefaultAsync(cancellationToken);

            var response = new UserFeaturesResponse();
            if (saPermission != null)
            {
                response.Apis.Add(new()
                {
                    Name = "API",
                    Endpoint = $"{webServerOptions.Endpoint}/api/api",
                    Features = ["Manage"]
                });
            }

            foreach (var api in apis)
            {
                LpApiFeaturesRequest request = null;
                if (saPermission != null)
                {
                    request = new() { Permissions = [ToFeaturesRequest(saPermission)] };
                }
                else
                {
                    var userApiPermissions = await dbContext.Permissions
                        .Include(p => p.BinaryValue)
                        .Include(p => p.LevelValue)
                        .Include(p => p.PasskeyValue)
                        .AsNoTracking()
                        .Where(p => p.ApiId == api.Id && userPermissionIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);
                    request = new()
                    {
                        Permissions = userApiPermissions.Select(ToFeaturesRequest).ToList()
                    };
                }

                var featuresResponse = await httpService
                    .LpApiRequestAsync<LpApiFeaturesResponsePayload>(
                        httpClient,
                        new()
                        {
                            Method = HttpMethod.Post,
                            BaseUrl = $"{api.CallbackUrl}/features",
                            Content = request
                        });
                response.Apis.Add(new()
                {
                    Name = api.Name,
                    Endpoint = api.Endpoint,
                    Features = featuresResponse.Features
                });
            }

            return response;
        }
        #endregion
        #endregion

        #region Internals
        private LpApiFeaturesRequest.Permission ToFeaturesRequest(AuthServerPermission p)
        {
            var passkeys = p.PasskeyValue.Select(c => c.Passkey).ToHashSet();
            return new LpApiFeaturesRequest.Permission
            {
                Type = authService.GetPermissionType(p.FullName),
                Name = authService.GetPermissionName(p.FullName),
                Allow = p.BinaryValue?.Allow,
                Level = p.LevelValue?.Level,
                Passkeys = passkeys.Count > 0 ? passkeys : null,
            };
        }
        #endregion
    }
}
