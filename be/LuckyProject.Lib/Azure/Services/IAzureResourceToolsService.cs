using System;

namespace LuckyProject.Lib.Azure.Services
{
    public interface IAzureResourceToolsService
    {
        Uri GetKeyVaultUrl(string vaultName);
    }
}
