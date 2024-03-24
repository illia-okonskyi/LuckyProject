using Azure.Identity;

namespace LuckyProject.Lib.Azure.Services
{
    public class AzureIdentityService : IAzureIdentityService
    {
        public ClientSecretCredential CreateClientCredential(
            string tenantId,
            string clientId,
            string clientSecret)
        {
            return new ClientSecretCredential(tenantId, clientId, clientSecret);
        }
    }
}
