using LuckyProject.ConsoleHostApp.Services.Dummy;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp.Services.Hosted
{
    public class LlHostedService : IHostedService
    {
        private readonly IHelloWorldService helloWorldService;
        private readonly ILogger logger;

        public LlHostedService(
            IHostApplicationLifetime appLifetime,
            IHelloWorldService helloWorldService,
            ILogger<LlHostedService> logger)
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
