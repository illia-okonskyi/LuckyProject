
namespace LuckyProject.SecretManager.Models
{
    public class AppConfig
    {
        public class KeyVaultConfig
        {
            public string Name { get; set; }
            public string TenantId { get; set; }
            public string ClientId { get; set; }
        }

        public KeyVaultConfig KeyVault {  get; set; }
    }
}
