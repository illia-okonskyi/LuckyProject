using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLpBasicServices(this IServiceCollection services)
        {
            services.AddSingleton<IStringService, StringService>();
            services.AddSingleton<IConsoleService, ConsoleService>();
            services.AddSingleton<IEnvironmentService, EnvironmentService>();
            services.AddSingleton<IRuntimeInformationService, RuntimeInformationService>();
            services.AddSingleton<IFsService, FsService>();
            services.AddSingleton<IXmlService, XmlService>();
            services.AddSingleton<IJsonService, JsonService>();
            services.AddSingleton<ICryptoService, CryptoService>();
            services.AddSingleton<IVersionService, VersionService>();
            return services;
        }

        public static IServiceCollection AddLpAppVersionService(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<AppVersionServiceOptions>(config);
            services.AddSingleton<IAppVersionService, AppVersionService>();
            return services;
        }
    }
}
