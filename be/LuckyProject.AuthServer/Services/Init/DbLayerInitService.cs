using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Models;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.Permissions;
using LuckyProject.AuthServer.Services.Roles;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Init
{
    public class DbLayerInitService : IDbLayerInitService
    {
        private readonly AuthServerDbContextOptions options;
        private readonly InitialSeedOptions initialSeed;
        private readonly WebServerOptions webServerOptions;
        private readonly IFsService fsService;
        private readonly AuthServerDbContext dbContext;
        private readonly IInitialSeedReader initialSeedReader;
        private readonly IRolesService rolesService;
        private readonly IUsersService usersService;
        private readonly IPermissionsService permissionsService;
        private readonly IOpenIdService openIdService;
        private readonly ILogger logger;

        public DbLayerInitService(
            IOptions<AuthServerDbContextOptions> options,
            IOptions<InitialSeedOptions> initialSeed,
            IOptions<WebServerOptions> webServerOptions,
            IFsService fsService,
            AuthServerDbContext dbContext,
            IInitialSeedReader initialSeedReader,
            IRolesService rolesService,
            IUsersService usersService,
            IPermissionsService permissionsService,
            IOpenIdService openIdService,
            ILogger<DbLayerInitService> logger)
        {
            this.options = options.Value;
            this.initialSeed = initialSeed.Value;
            this.webServerOptions = webServerOptions.Value;
            this.fsService = fsService;
            this.dbContext = dbContext;
            this.initialSeedReader = initialSeedReader;
            this.rolesService = rolesService;
            this.usersService = usersService;
            this.permissionsService = permissionsService;
            this.openIdService = openIdService;
            this.logger = logger;
        }

        public async Task InitDbAsync()
        {
            using var loggerScope = logger.BeginScope("DbInit");

            logger.LogInformation("Ensuring db dir created...");
            fsService.DirectoryEnsureCreated(options.DbDirectory);
            logger.LogInformation("Migrating db...");
            await dbContext.Database.MigrateAsync();
            await SeedAsync();
        }

        private async Task SeedAsync()
        {
            var seedTimeUtc = await initialSeedReader.GetSeedTimeUtcAsync();
            if (seedTimeUtc.HasValue)
            {
                logger.LogInformation("Seeded at {Value:o} UTC", seedTimeUtc.Value);
                return;
            }

            logger.LogInformation("Seeding...");
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                logger.LogInformation("Seeding SA...");
                var saPermission = await permissionsService.CreatePermissionAsync(new()
                {
                    Type = LpAuthPermissionType.Root,
                    Name = initialSeed.Sa.PermissionName,
                    Description = "Super Admin Permission",
                    IsSealed = true
                });
                var saRole = await rolesService.CreateRoleAsync(new()
                {
                    Name = initialSeed.Sa.RoleName,
                    Description = "Super Admin Role",
                    IsSealed = true
                });
                var saUser = await usersService.CreateWebUserAsync(new()
                {
                    UserName = initialSeed.Sa.UserName,
                    Email = initialSeed.Sa.Email,
                    PhoneNumber = initialSeed.Sa.PhoneNumber,
                    FullName = initialSeed.Sa.FullName,
                    TelegramUserName = initialSeed.Sa.TelegramUserName,
                    PreferredLocale = initialSeed.Sa.PreferredLocale,
                    Password = initialSeed.Sa.Password,
                    IsSealed = true
                });
                await rolesService.AssignPermissionToRoleAsync(new()
                {
                    RoleId = saRole.Id,
                    PermissionId = saPermission.Id,
                    IgnoreSealed = true
                });
                await usersService.AssignRoleToUserAsync(new()
                {
                    RoleId = saRole.Id,
                    UserId = saUser.Id,
                    IgnoreSealed = true
                });

                logger.LogInformation("Seeding AuthServer API...");
                var authServerApiPermission = await permissionsService.CreatePermissionAsync(new()
                {
                    Type = LpAuthPermissionType.Binary,
                    Name = initialSeed.AuthServerApi.PermissionName,
                    Description = "AuthServer API Permission",
                    Allow = true,
                    IsSealed = true
                });
                var authServerApiRole = await rolesService.CreateRoleAsync(new()
                {
                    Name = initialSeed.AuthServerApi.RoleName,
                    Description = "AuthServer API Users Role",
                    IsSealed = true
                });
                await rolesService.AssignPermissionToRoleAsync(new()
                {
                    RoleId = authServerApiRole.Id,
                    PermissionId = authServerApiPermission.Id,
                    IgnoreSealed = true
                });

                logger.LogInformation("Seeding Clients...");
                var defaultWebClient = await openIdService.CreateWebClientAsync(new()
                {
                    Name = "lp-default",
                    DisplayName = "Lucky Project Default Web Client",
                    Origins = new()
                    {
                        new(initialSeed.DefaultWebClient.BaseUrl,
                            initialSeed.DefaultWebClient.RedirectPath,
                            initialSeed.DefaultWebClient.PostLogoutRedirectPath)
                    },
                    IsSealed = true
                });

                var swaggerWebClient = await openIdService.CreateWebClientAsync(new()
                {
                    Name = "lp-swagger",
                    DisplayName = "Lucky Project Swagger Web Client",
                    Origins = new()
                    {
                        new(webServerOptions.EndpointUri, "/swagger/oauth2-redirect.html")
                    },
                    IsSealed = true
                });

                var utcNow = DateTime.UtcNow;
                dbContext.InitialSeed.Add(new() { SeedTimeUtc = utcNow });
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.LogInformation("Seed done at {utcNow:o} UTC", utcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");
                await transaction.RollbackAsync();
            }
        }
    }
}
