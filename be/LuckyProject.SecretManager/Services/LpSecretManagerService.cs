using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Azure.Services;
using LuckyProject.Lib.ConsoleTool.Helpers;
using LuckyProject.Lib.Hosting.HostedServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Azure.Identity;

namespace LuckyProject.SecretManager.Services
{
    internal class LpSecretManagerService : AbstractLpSingleRunLpHostedService
    {
        #region Definitions & constants
        private class SecretInfo
        {
            public string Name { get; init; }
            public string DisplayName { get; init; }
            public string Value { get; set; }
        }

        private const string OutDir = "out";
        private const string OutFileName = "secrets.txt";
        #endregion

        #region Services
        private readonly LpSecretManagerServiceOptions options;
        private readonly ILogger logger;

        private readonly IAppVersionService appVersionService;
        private readonly IConsoleService consoleService;
        private readonly IAzureIdentityService azureIdentityService;
        private readonly IAzureKeyVaultService azureKeyVaultService;
        private readonly IFsService fsService;
        #endregion

        #region Data
        private readonly List<SecretInfo> secrets;
        #endregion

        #region ctor
        public LpSecretManagerService(
            IOptions<LpSecretManagerServiceOptions> options,
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            ILogger<LpSecretManagerService> logger)
            : base(
                  serviceScopeService,
                  appLifetime,
                  environmentService,
                  timerFactory,
                  logger,
                  TimeoutDefaults.Short)
        {
            this.options = options.Value;
            this.logger = logger;
            appVersionService = ServiceProvider.GetRequiredService<IAppVersionService>();
            consoleService = ServiceProvider.GetRequiredService<IConsoleService>();
            azureIdentityService = ServiceProvider.GetRequiredService<IAzureIdentityService>();
            azureKeyVaultService = ServiceProvider.GetRequiredService<IAzureKeyVaultService>();
            fsService = ServiceProvider.GetRequiredService<IFsService>();

            secrets = new()
            {
                new()
                {
                    Name = "Tools-CertManager",
                    DisplayName = "Tools.CertManager"
                }
            };
        }
        #endregion

        #region Execute
        protected override async Task ExecuteSingleRunServiceAsync(
            CancellationToken cancellationToken)
        {
            await appVersionService.InitAsync();
            var args = EnvironmentService.GetCommandLineArgs();
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

            throw new LpExitCodeException(ExitCodes.ArgumentError, "Unexpected command");
        }
        #endregion

        #region Internals
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

            logger.LogInformation("Retrieving Secrets...");
            var credential = azureIdentityService.CreateClientCredential(
                options.TenantId,
                options.ClientId,
                masterPassword);
            var client = azureKeyVaultService.CreateSecretClient(options.KeyVaultName, credential);
            foreach (var secret in secrets)
            {
                using var logScope = logger.BeginScope("Get Secret {Name}", secret.Name);
                try
                {
                    secret.Value = await azureKeyVaultService.GetSecretAsync(
                        client,
                        secret.Name,
                        cancellationToken);
                    logger.LogInformation($"OK");
                }
                catch (AuthenticationFailedException)
                {
                    logger.LogError("Wrong master password");
                    throw new LpExitCodeException(ExitCodes.AuthFailed, "Wrong master password");
                }
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
            foreach (var secret in secrets)
            {
                sb.Append("- ");
                sb.Append(secret.DisplayName);
                sb.Append(": ");
                sb.AppendLine(secret.Value);
            }
            sb.AppendLine();
            return sb.ToString();
        }
        #endregion
    }
}
