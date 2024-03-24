using Azure.Identity;
using Microsoft.Extensions.Options;

namespace LuckyProject.Lib.Azure.Services
{
    public class AzureAppRegistrationServiceOptions
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class AzureAppRegistrationService : IAzureAppRegistrationService
    {
        public ClientSecretCredential Credential { get; }

        public AzureAppRegistrationService(
            IOptions<AzureAppRegistrationServiceOptions> options,
            IAzureIdentityService identityService)
        {
            Credential = identityService.CreateClientCredential(
                options.Value.TenantId,
                options.Value.ClientId,
                options.Value.ClientSecret);
        }
    }
}
