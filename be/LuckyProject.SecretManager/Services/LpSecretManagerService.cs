using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Azure.Services;
using LuckyProject.Lib.ConsoleTool.Helpers;

namespace LuckyProject.SecretManager.Services
{
    internal class LpSecretManagerService : IHostedService
    {
        private class SecretInfo
        {
            public string Name { get; init; }
            public string DisplayName { get; init; }
            public string Value { get; set; }
        }

        private const string OutDir = "out";
        private const string OutFileName = "secrets.txt";

        private readonly LpSecretManagerServiceOptions options;
        private readonly IAppVersionService appVersionService;
        private readonly IEnvironmentService environmentService;
        private readonly IConsoleService consoleService;
        private readonly IAzureIdentityService azureIdentityService;
        private readonly IAzureKeyVaultService azureKeyVaultService;
        private readonly IFsService fsService;
        private readonly ILogger logger;

        private readonly List<SecretInfo> secrets;

        public LpSecretManagerService(
            IOptions<LpSecretManagerServiceOptions> options,
            IAppVersionService appVersionService,
            IEnvironmentService environmentService,
            IConsoleService consoleService,
            IAzureIdentityService azureIdentityService,
            IAzureKeyVaultService azureKeyVaultService,
            IFsService fsService,
            ILogger<LpSecretManagerService> logger)
        {
            this.options = options.Value;
            this.appVersionService = appVersionService;
            this.environmentService = environmentService;
            this.consoleService = consoleService;
            this.azureIdentityService = azureIdentityService;
            this.azureKeyVaultService = azureKeyVaultService;
            this.fsService = fsService;
            this.logger = logger;

            secrets = new()
            {
                new()
                {
                    Name = "Tools-CertManager",
                    DisplayName = "Tools.CertManager"
                }
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HostedServiceHelper.ExecuteAsync(
                ExecuteAsync,
                environmentService,
                logger,
                cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await appVersionService.InitAsync();
            var args = environmentService.GetCommandLineArgs();
            if (CommandLineArgsHelper.IsHelpRequested(args))
            {
                WriteHelp();
                return;
            }

            var command = args[1];
            if (command == "get-secrets")
            {
                await GetSecretsAsync(cancellationToken);
                return;
            }

            throw new LpConsoleAppErrorException(1, "Unexpected command");
        }

        private void WriteHelp()
        {
            consoleService.WriteLine(
                $"LuckyProject.SecretManager version {appVersionService.AppVersion}");
            consoleService.WriteLine();
            consoleService.WriteLine("Usage:");
            consoleService.WriteLine("1) Display help message:");
            consoleService.WriteLine("   <APP>");
            consoleService.WriteLine("   <APP> -h");
            consoleService.WriteLine("   <APP> --help");
            consoleService.WriteLine("2) Obtain Secrets:");
            consoleService.WriteLine("   <APP> get-secrets");
        }

        private async Task GetSecretsAsync(CancellationToken cancellationToken)
        {
            consoleService.Write("Enter Master Password:");
            var masterPassword = consoleService.ReadLine();

            logger.LogInformation($"Retrieving Secrets...");
            var credential = azureIdentityService.CreateClientCredential(
                options.TenantId,
                options.ClientId,
                masterPassword);
            var client = azureKeyVaultService.CreateSecretClient(options.KeyVaultName, credential);
            foreach (var secret in secrets)
            {
                logger.LogInformation($"Retrieving Secret: {secret.DisplayName}...");
                secret.Value = await azureKeyVaultService.GetSecretAsync(
                    client,
                    secret.Name,
                    cancellationToken);
                logger.LogInformation($"OK");
            }

            fsService.DirectoryEnsureCreated(OutDir);
            var outFilePath = fsService.PathCombine(OutDir, OutFileName);
            await fsService.FileWriteAllTextAsync(
                outFilePath,
                BuildOutFileContents(),
                cancellationToken);
            logger.LogInformation($"Secrets written to: {outFilePath}");
        }

        private string BuildOutFileContents()
        {
            var sb = new StringBuilder("LuckyProject Secrets:");
            sb.AppendLine();
            sb.AppendLine();
            foreach(var secret in secrets)
            {
                sb.Append("- ");
                sb.Append(secret.DisplayName);
                sb.Append(": ");
                sb.AppendLine(secret.Value);
            }
            sb.AppendLine();
            return sb.ToString();
        }
    }
}
