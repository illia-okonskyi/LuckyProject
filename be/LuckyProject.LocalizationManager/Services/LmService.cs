using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.EntryPoint;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LuckyProject.LocalizationManager.Services
{
    internal class LmService : WinUiHostedService<App>
    {
        #region ctor
        public LmService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            ILogger<LmService> logger)
            : base(
                  serviceScopeService,
                  appLifetime,
                  environmentService,
                  timerFactory,
                  logger)
        { }
        #endregion
    }
}
