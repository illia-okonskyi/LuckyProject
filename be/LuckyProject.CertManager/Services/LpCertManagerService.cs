using LuckyProject.Lib.Azure.Services;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.ConsoleTool.Helpers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.CertManager.Services
{
    internal class LpCertManagerService : IHostedService
    {
        private const string DevCaCertName = "DevCA";
        private const string DevCaCertOutName = "DevCA.crt";
        private const string DevCertName = "cert-dev";
        private const string DevCertOutName = "cert-dev.pfx";
        private const string OutDir = "out";

        private readonly LpCertManagerServiceOptions options;
        private readonly IAppVersionService appVersionService;
        private readonly IEnvironmentService environmentService;
        private readonly IConsoleService consoleService;
        private readonly IAzureAppRegistrationService azureAppRegistrationService;
        private readonly IAzureKeyVaultService azureKeyVaultService;
        private readonly IFsService fsService;
        private readonly ILogger logger;

        public LpCertManagerService(
            IOptions<LpCertManagerServiceOptions> options,
            IAppVersionService appVersionService,
            IEnvironmentService environmentService,
            IConsoleService consoleService,
            IAzureAppRegistrationService azureAppRegistrationService,
            IAzureKeyVaultService azureKeyVaultService,
            IFsService fsService,
            ILogger<LpCertManagerService> logger)
        {
            this.options = options.Value;
            this.appVersionService = appVersionService;
            this.environmentService = environmentService;
            this.consoleService = consoleService;
            this.azureAppRegistrationService = azureAppRegistrationService;
            this.azureKeyVaultService = azureKeyVaultService;
            this.fsService = fsService;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HostedServiceHelper.ExecuteAsync(
                ExecuteAsync,
                environmentService,
                logger,
                cancellationToken,
                ec => consoleService.ReadKey());
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
            if (command == "get-dev-ca-cert")
            {
                await GetDevCaCertAsync(cancellationToken);
                return;
            }

            if (command == "get-dev-cert")
            {
                await GetDevCertAsync(cancellationToken);
                return;
            }

            throw new LpConsoleAppErrorException(1, "Unexpected command");
        }

        private void WriteHelp()
        {
            consoleService.WriteLine(
                $"LuckyProject.CertManager version {appVersionService.AppVersion}");
            consoleService.WriteLine();
            consoleService.WriteLine("Usage:");
            consoleService.WriteLine("1) Display help message:");
            consoleService.WriteLine("   <APP>");
            consoleService.WriteLine("   <APP> -h");
            consoleService.WriteLine("   <APP> --help");
            consoleService.WriteLine("2) Obtain Developer CA certificate:");
            consoleService.WriteLine("   <APP> get-dev-ca-cert");
            consoleService.WriteLine("3) Obtain Developer certificate:");
            consoleService.WriteLine("   <APP> get-dev-cert");
        }

        private async Task GetDevCaCertAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Retrieving Developer CA Certificate...");
            var certClient = azureKeyVaultService.CreateCertificateClient(
                options.KeyVaultName,
                azureAppRegistrationService.Credential);
            using var cert = await azureKeyVaultService.GetCertificateAsync(
                certClient,
                DevCaCertName,
                cancellationToken);
            logger.LogInformation("Thumbprint: {Thumbprint}", cert.Thumbprint);

            fsService.DirectoryEnsureCreated(OutDir);
            var certOutPath = fsService.PathCombine(OutDir, DevCaCertOutName);
            await fsService.FileWriteAllTextAsync(
                certOutPath,
                cert.ExportCertificatePem(),
                cancellationToken);
            logger.LogInformation("Developer CA Certificate written to: {certOutPath}", certOutPath);
        }

        private async Task GetDevCertAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Retrieving Developer Certificate...");
            var certClient = azureKeyVaultService.CreateCertificateClient(
                options.KeyVaultName,
                azureAppRegistrationService.Credential);
            var secretClient = azureKeyVaultService.CreateSecretClient(
                options.KeyVaultName,
                azureAppRegistrationService.Credential);
            using var cert = await azureKeyVaultService.GetCertificateWithPrivateKeyAsync(
                certClient,
                secretClient,
                DevCertName,
                cancellationToken);
            logger.LogInformation("Thumbprint: {Thumbprint}", cert.Thumbprint);

            fsService.DirectoryEnsureCreated(OutDir);
            var certOutPath = fsService.PathCombine(OutDir, DevCertOutName);
            await fsService.FileWriteAllBytesAsync(
                certOutPath,
                cert.Export(X509ContentType.Pkcs12),
                cancellationToken);
            logger.LogInformation("Developer Certificate written to: {certOutPath}", certOutPath);
        }
    }
}
