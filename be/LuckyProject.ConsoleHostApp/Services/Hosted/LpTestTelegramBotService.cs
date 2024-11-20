using Azure.Communication.Email;
using LuckyProject.Lib.Azure.Services;
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
    public class LpTestTelegramBotService : AbstractLpSingleRunLpHostedService
    {
        private readonly IConsoleService consoleService;
        private readonly ILogger logger;

        private readonly IAzureCsService azureCsService;

        public LpTestTelegramBotService(
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
            consoleService = ServiceProvider.GetRequiredService<IConsoleService>();
            azureCsService = ServiceProvider.GetRequiredService<IAzureCsService>();
            this.logger = logger;
        }

        protected override async Task ExecuteSingleRunServiceAsync(CancellationToken cancellationToken)
        {
            try
            {
                var r = await azureCsService.SendEmailAsync(
                    new EmailAddress("illia.okonskyi.work@gmail.com"),
                    "Testing the service",
                    @"
		            <html>
			            <body>
				            <h1>Some text</h1>
			            </body>
		            </html>");
                consoleService.WriteLine($"!!! RESULT = {r}");
            }
            catch (Exception ex)
            {
                // Exception Details
                int a = 9;
            }

            consoleService.WriteLine("!!!Mail sent");
        }

        protected override async Task<int?> OnBeforeServiceExitBaseAsync(int exitCode)
        {
            consoleService.WriteLine("Application is about to exit. Press Any Key to exit");
            consoleService.ReadKey();
            return null;
        }
    }
}
