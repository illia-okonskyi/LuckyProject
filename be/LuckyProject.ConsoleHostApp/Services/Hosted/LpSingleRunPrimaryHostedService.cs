using LuckyProject.ConsoleHostApp.Services.Dummy;
using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Hosting.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp.Services.Hosted
{
    public class LpSingleRunPrimaryHostedService : AbstractLpSingleRunLpHostedService
    {
        private readonly IHelloWorldService helloWorldService;
        private readonly IConsoleService consoleService;
        private readonly ILogger logger;

        public LpSingleRunPrimaryHostedService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            ILogger<LpSingleRunPrimaryHostedService> logger)
            : base(
                  serviceScopeService,
                  appLifetime,
                  environmentService,
                  timerFactory,
                  "PrimaryService",
                  logger)
        {
            helloWorldService = ServiceProvider.GetRequiredService<IHelloWorldService>();
            consoleService = ServiceProvider.GetRequiredService<IConsoleService>();
            this.logger = logger;
        }

        protected override async Task ExecuteSingleRunServiceAsync(CancellationToken cancellationToken)
        {
            helloWorldService.SayHello(Name);
            await TaskHelper.SafeDelay(TimeSpan.FromSeconds(10));
            RequestStopApplication();
        }

        protected override Task<int?> OnBeforeServiceExitBaseAsync(int exitCode)
        {
            consoleService.WriteLine("Application is about to exit. Press Any Key to exit");
            consoleService.ReadKey();
            return Task.FromResult<int?>(null);
        }
    }
}
