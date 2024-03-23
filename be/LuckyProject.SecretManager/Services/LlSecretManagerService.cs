using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using LuckyProject.SecretManager.Models;
using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace LuckyProject.SecretManager.Services
{
    internal class LlSecretManagerService : IHostedService
    {
        private class SecretInfo
        {
            public string Name { get; init; }
            public string DisplayName { get; init; }
            public string Value { get; set; }
        }

        private const string OutDir = "out";
        private const string OutFileName = "secrets.txt";

        private readonly AppConfig appConfig;
        private readonly ILogger logger;

        private readonly List<SecretInfo> secrets;

        public LlSecretManagerService(
            IOptions<AppConfig> appConfig,
            ILogger<LlSecretManagerService> logger)
        {
            this.appConfig = appConfig.Value;
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
            var exitCode = 0;
            try
            {
                await ExecuteAsync(cancellationToken);
            }
            catch (LlConsoleAppErrorException appErrorEx)
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
            if (command == "get-secrets")
            {
                await GetSecretsAsync(cancellationToken);
                return;
            }

            throw new LlConsoleAppErrorException(1, "Unexpected command");
        }

        private void WriteHelp()
        {
            Console.WriteLine("LuckyProject.SecretManager version 0.0.1");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("1) Display help message:");
            Console.WriteLine("   <APP>");
            Console.WriteLine("   <APP> -h");
            Console.WriteLine("   <APP> --help");
            Console.WriteLine("2) Obtain Secrets:");
            Console.WriteLine("   <APP> get-secrets");
        }

        private async Task GetSecretsAsync(CancellationToken cancellationToken)
        {
            Console.Write("Enter Master Password:");
            var masterPassword = Console.ReadLine();
            logger.LogInformation($"Retrieving Secrets...");
            var vaultUrl = GetVaultUrl();
            var clientCredential = GetClientCredential(masterPassword);
            var client = new SecretClient(vaultUrl, clientCredential);
            foreach (var secret in secrets)
            {
                await GetSecretAsync(client, secret, cancellationToken);
            }
            EnsureOutDir();
            var outFilePath = Path.Combine(OutDir, OutFileName);
            await File.WriteAllTextAsync(outFilePath, BuildOutFileContents(), cancellationToken);
            logger.LogInformation($"Secrets written to: {outFilePath}");
        }

        private Uri GetVaultUrl()
        {
            return new Uri($"https://{appConfig.KeyVault.Name}.vault.azure.net");
        }

        private ClientSecretCredential GetClientCredential(string masterPassword)
        {
            return new ClientSecretCredential(
                appConfig.KeyVault.TenantId,
                appConfig.KeyVault.ClientId,
                masterPassword);
        }

        private void EnsureOutDir()
        {
            if (Directory.Exists(OutDir))
            {
                return;
            }

            Directory.CreateDirectory(OutDir);
        }

        private async Task GetSecretAsync(
            SecretClient client,
            SecretInfo secret,
            CancellationToken cancellationToken)
        {
            logger.LogInformation($"Retrieving Secret: {secret.DisplayName}...");
            var response = await client.GetSecretAsync(
                secret.Name,
                cancellationToken: cancellationToken);
            secret.Value = response.Value.Value;
            logger.LogInformation($"OK");
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
