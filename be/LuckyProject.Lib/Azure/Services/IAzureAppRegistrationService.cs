using Azure.Identity;

namespace LuckyProject.Lib.Azure.Services
{
    public interface IAzureAppRegistrationService
    {
        public ClientSecretCredential Credential { get; }
    }
}
