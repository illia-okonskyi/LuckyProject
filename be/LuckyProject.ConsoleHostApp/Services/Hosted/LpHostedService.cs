using LuckyProject.ConsoleHostApp.Services.Dummy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp.Services.Hosted
{
    public class LpHostedService : IHostedService
    {
        private readonly IHelloWorldService helloWorldService;
        private readonly ILogger logger;

        public LpHostedService(
            IHostApplicationLifetime appLifetime,
            IHelloWorldService helloWorldService,
            ILogger<LpHostedService> logger)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            this.helloWorldService = helloWorldService;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("1. StartAsync has been called.");
            logger.LogInformation("1");
            var s1 = logger.BeginScope("S1");
            logger.LogInformation("2");
            var s2 = logger.BeginScope("S2");
            logger.LogInformation("3");
            s2.Dispose();
            logger.LogInformation("4");
            s1.Dispose();
            logger.LogInformation("5");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("4. StopAsync has been called.");

            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            logger.LogInformation("2. OnStarted has been called.");
            helloWorldService.SayHello();
        }

        private void OnStopping()
        {
            logger.LogInformation("3. OnStopping has been called.");
        }

        private void OnStopped()
        {
            logger.LogInformation("5. OnStopped has been called.");
        }
    }
}
