using System;

namespace LuckyProject.Lib.Azure.Services
{
    public class AzureResourceToolsService : IAzureResourceToolsService
    {
        public Uri GetKeyVaultUrl(string vaultName)
        {
            return new Uri($"https://{vaultName}.vault.azure.net");
        }
    }
}
