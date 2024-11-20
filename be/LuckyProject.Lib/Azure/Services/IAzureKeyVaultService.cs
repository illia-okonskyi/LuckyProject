using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Azure.Security.KeyVault.Secrets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Azure.Services
{
    public interface IAzureKeyVaultService
    {
        #region Factory methods
        CertificateClient CreateCertificateClient(
            string vaultName,
            ClientSecretCredential credential);
        SecretClient CreateSecretClient(
            string vaultName,
            ClientSecretCredential credential);
        #endregion

        #region GetCertificate
        Task<X509Certificate2> GetCertificateAsync(
            CertificateClient certClient,
            string certName,
            CancellationToken cancellationToken = default);
        Task<X509Certificate2> GetCertificateWithPrivateKeyAsync(
            CertificateClient certClient,
            SecretClient secretClient,
            string certName,
            CancellationToken cancellationToken = default);
        #endregion

        #region GetSecret
        Task<string> GetSecretAsync(
             SecretClient secretClient,
             string secretName,
             CancellationToken cancellationToken = default);
        #endregion
    }
}
