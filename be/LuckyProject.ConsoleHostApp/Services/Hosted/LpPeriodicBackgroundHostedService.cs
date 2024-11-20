using LuckyProject.ConsoleHostApp.Services.Dummy;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Hosting.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp.Services.Hosted
{
    public class LpPeriodicBackgroundHostedService : AbstractLpPeriodicLpHostedService
    {
        private readonly IHelloWorldService helloWorldService;
        private int execCount;

        public LpPeriodicBackgroundHostedService(
            IServiceScopeService serviceScopeService,
            ILpTimerFactory timerFactory,
            ILogger<LpPeriodicBackgroundHostedService> logger)
            : base(
                  serviceScopeService,
                  timerFactory,
                  TimeSpan.FromSeconds(1),
                  "BackgroundService",
                  logger)
        {
            helloWorldService = ServiceProvider.GetRequiredService<IHelloWorldService>();
        }

        protected override Task ExecutePeriodicServiceAsync(
            bool isFirstRun,
            CancellationToken cancellationToken)
        {
            helloWorldService.SayHello($"{Name} #{++execCount}");
            return Task.CompletedTask;
        }
    }
}
