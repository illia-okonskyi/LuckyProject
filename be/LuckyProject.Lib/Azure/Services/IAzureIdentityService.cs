using Azure.Identity;

namespace LuckyProject.Lib.Azure.Services
{
    public interface IAzureIdentityService
    {
        ClientSecretCredential CreateClientCredential(
            string tenantId,
            string clientId,
            string clientSecret);
    }
}
