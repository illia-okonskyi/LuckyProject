using LuckyProject.AuthServer.Services.Init;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Hosting.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services
{
    internal class AuthServerService : AbstractLpSingleRunLpHostedService
    {
        #region Services
        private readonly IAppVersionService appVersionService;
        private readonly IDbLayerInitService dbLayerInitService;
        private readonly ILpLocalizationService localizationService;
        private readonly IAuthSessionManager authSessionManager;
        private readonly IUsersLoginService usersLoginService;
        private readonly ILogger logger;
        #endregion

        #region ctor
        public AuthServerService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            ILogger<AuthServerService> logger)
            : base(
                  serviceScopeService,
                  appLifetime,
                  environmentService,
                  timerFactory,
                  nameof(AuthServerService),
                  logger)
        {
            this.logger = logger;

            appVersionService = ServiceProvider.GetRequiredService<IAppVersionService>();
            dbLayerInitService = ServiceProvider.GetRequiredService<IDbLayerInitService>();
            localizationService = ServiceProvider.GetRequiredService<ILpLocalizationService>();
            authSessionManager = ServiceProvider.GetRequiredService<IAuthSessionManager>();
            usersLoginService = ServiceProvider.GetRequiredService<IUsersLoginService>();
        }
        #endregion

        #region Start/Stop/Execute
        protected override async Task StartSingleRunServiceAsync(
            CancellationToken cancellationToken)
        {
            await appVersionService.InitAsync();
            await dbLayerInitService.InitDbAsync();

            var availableLocales = localizationService.GetAvailableLocales(new()
            {
                "lp-authserver-be",
                "lp-authserver-fe-errors",
                "lp-authserver-fe-ui",
                "lp-validation"
            });
            await localizationService.SetLocalesAsync(
                availableLocales.Select(l => l.Name).ToHashSet());
            await localizationService.LoadSourceAsync("lp-authserver-be");

            await authSessionManager.StartAsync();
            usersLoginService.Start();
        }

        protected override async Task StopSingleRunServiceAsync(CancellationToken cancellationToken)
        {
            await usersLoginService.StopAsync();
            await authSessionManager.StopAsync();
        }
        #endregion
    }
}
