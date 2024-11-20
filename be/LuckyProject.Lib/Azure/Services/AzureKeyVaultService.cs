using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using LuckyProject.Lib.Basics.Services;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Azure.Services
{
    public class AzureKeyVaultService : IAzureKeyVaultService
    {
        #region Internals
        private readonly IAzureResourceToolsService resourceToolsService;
        private readonly IStringService stringService;
        #endregion

        #region ctor
        public AzureKeyVaultService(
            IAzureResourceToolsService resourceToolsService,
            IStringService stringService)
        {
            this.resourceToolsService = resourceToolsService;
            this.stringService = stringService;
        }
        #endregion

        #region Factory methods
        public CertificateClient CreateCertificateClient(
            string vaultName,
            ClientSecretCredential credential)
        {
            return new CertificateClient(
                resourceToolsService.GetKeyVaultUrl(vaultName),
                credential);
        }

        public SecretClient CreateSecretClient(
            string vaultName,
            ClientSecretCredential credential)
        {
            return new SecretClient(
                resourceToolsService.GetKeyVaultUrl(vaultName),
                credential);
        }
        #endregion

        #region GetCertificate
        public async Task<X509Certificate2> GetCertificateAsync(
            CertificateClient certClient,
            string certName,
            CancellationToken cancellationToken = default)
        {
            var response = await certClient.GetCertificateAsync(certName, cancellationToken);
            return new X509Certificate2(
                response.Value.Cer,
                string.Empty,
                X509KeyStorageFlags.Exportable);
        }

        public async Task<X509Certificate2> GetCertificateWithPrivateKeyAsync(
            CertificateClient certClient,
            SecretClient secretClient,
            string certName,
            CancellationToken cancellationToken = default)
        {
            var certResponse = await certClient.GetCertificateAsync(certName, cancellationToken);
            var secretName = certResponse.Value.SecretId.Segments[2].TrimEnd('/');
            var secret = await GetSecretAsync(secretClient, secretName, cancellationToken);
            return new X509Certificate2(
                stringService.FromBase64String(secret),
                string.Empty,
                X509KeyStorageFlags.Exportable);
        }
        #endregion

        #region GetSecret
        public async Task<string> GetSecretAsync(
             SecretClient secretClient,
             string secretName,
             CancellationToken cancellationToken = default)
        {
            var response = await secretClient.GetSecretAsync(
                secretName,
                cancellationToken: cancellationToken);
            return response.Value.Value;
        }
        #endregion
    }
}
