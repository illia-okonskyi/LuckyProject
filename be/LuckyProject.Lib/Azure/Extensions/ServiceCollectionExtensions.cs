using LuckyProject.Lib.Azure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LuckyProject.Lib.Azure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLpAzureBasicServices(this IServiceCollection services)
        {
            services.AddSingleton<IAzureIdentityService, AzureIdentityService>();
            services.AddSingleton<IAzureResourceToolsService, AzureResourceToolsService>();
            services.AddSingleton<IAzureKeyVaultService, AzureKeyVaultService>();
            return services;
        }

        public static IServiceCollection AddLpAzureAppRegistrationService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AzureAppRegistrationServiceOptions>(configuration);
            services.AddSingleton<IAzureAppRegistrationService, AzureAppRegistrationService>();
            return services;
        }

        public static IServiceCollection AddLpAzureCsService(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<AzureCsServiceOptions>(configuration);
            services.AddSingleton<IAzureCsService, AzureCsService>();
            return services;
        }
    }
}
