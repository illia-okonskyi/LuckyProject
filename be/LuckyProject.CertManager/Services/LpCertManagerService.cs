using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using LuckyProject.CertManager.Models;
using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
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

        private readonly AppConfig appConfig;
        private readonly AppSecretsConfig appSecrets;
        private readonly ILogger logger;

        public LpCertManagerService(
            IOptions<AppConfig> appConfig,
            IOptions<AppSecretsConfig> appSecrets,
            ILogger<LpCertManagerService> logger)
        {
            this.appConfig = appConfig.Value;
            this.appSecrets = appSecrets.Value;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var exitCode = 0;
            try
            {
                await ExecuteAsync(cancellationToken);
            }
            catch (LpConsoleAppErrorException appErrorEx)
            {
                exitCode = appErrorEx.ExitCode;
                logger.LogError(appErrorEx, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                exitCode = -1;
            }
            finally
            {
                Environment.Exit(exitCode);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length < 2 || args.Contains("-h") || args.Contains("--help"))
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
            Console.WriteLine("LuckyProject.CertManager version 0.0.1");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("1) Display help message:");
            Console.WriteLine("   <APP>");
            Console.WriteLine("   <APP> -h");
            Console.WriteLine("   <APP> --help");
            Console.WriteLine("2) Obtain Developer CA certificate:");
            Console.WriteLine("   <APP> get-dev-ca-cert");
            Console.WriteLine("3) Obtain Developer certificate:");
            Console.WriteLine("   <APP> get-dev-cert");
        }

        private async Task GetDevCaCertAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Retrieving Developer CA Certificate...");
            var client = new CertificateClient(GetVaultUrl(), GetClientCredential());
            var response = await client.GetCertificateAsync(DevCaCertName, cancellationToken);
            logger.LogInformation($"Thumbprint: {response.Value.Properties.X509ThumbprintString}");
            using var cert = new X509Certificate2(
                response.Value.Cer,
                string.Empty,
                X509KeyStorageFlags.Exportable);
            var pem = cert.ExportCertificatePem();
            EnsureOutDir();
            var certOutPath = Path.Combine(OutDir, DevCaCertOutName);
            await File.WriteAllTextAsync(certOutPath, pem, cancellationToken);
            logger.LogInformation($"Developer CA Certificate written to: {certOutPath}");
        }

        private async Task GetDevCertAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"Retrieving Developer Certificate...");
            var vaultUrl = GetVaultUrl();
            var clientCredential = GetClientCredential();
            var cerClient = new CertificateClient(vaultUrl, clientCredential);
            var certResponse = await cerClient.GetCertificateAsync(DevCertName, cancellationToken);
            logger.LogInformation($"Thumbprint: {certResponse.Value.Properties.X509ThumbprintString}"); logger.LogInformation($"Retrieving Developer Certificate Secret...");
            var secretName = certResponse.Value.SecretId.Segments[2].TrimEnd('/');
            var secretClient = new SecretClient(vaultUrl, clientCredential);
            var secretResponse = await secretClient.GetSecretAsync(
                secretName,
                cancellationToken: cancellationToken);
            using var cert = new X509Certificate2(
                Convert.FromBase64String(secretResponse.Value.Value),
                string.Empty,
                X509KeyStorageFlags.Exportable);
            var certBytes = cert.Export(X509ContentType.Pkcs12);
            EnsureOutDir();
            var certOutPath = Path.Combine(OutDir, DevCertOutName);
            await File.WriteAllBytesAsync(certOutPath, certBytes, cancellationToken);
            logger.LogInformation($"Developer Certificate written to: {certOutPath}");
        }

        private Uri GetVaultUrl()
        {
            return new Uri($"https://{appConfig.KeyVault.Name}.vault.azure.net");
        }

        private ClientSecretCredential GetClientCredential()
        {
            return new ClientSecretCredential(
                appConfig.KeyVault.TenantId,
                appConfig.KeyVault.ClientId,
                appSecrets.AppSecret);
        }

        private void EnsureOutDir()
        {
            if (Directory.Exists(OutDir))
            {
                return;
            }

            Directory.CreateDirectory(OutDir);
        }
    }
}
