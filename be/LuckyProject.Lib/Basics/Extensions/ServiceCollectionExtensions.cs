using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Basics.Services.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            services.AddSingleton<IThreadSyncService, ThreadSyncService>();
            services.AddSingleton<ILpTimerFactory, LpTimerFactory>();
            services.AddSingleton<ILpWorkerFactory, LpWorkerService>();
            services.AddSingleton<ILpWorkerService, LpWorkerService>();
            services.AddSingleton<IServiceScopeService, ServiceScopeService>();
            services.AddSingleton<ILpInMemoryMessageBus, LpInMemoryMessageBus>();
            services.AddSingleton<ICompressionService, CompressionService>();
            services.AddSingleton<IValidationService, ValidationService>();
            services.AddSingleton<ILpAuthorizationService, LpAuthorizationService>();
            services.AddSingleton<IJsonDocumentService, JsonDocumentService>();
            services.AddSingleton<ILpProcessFactory, LpProcessService>();
            services.AddSingleton<ILpProcessService, LpProcessService>();
            services.AddSingleton<ILpInMemoryCacheFactory, LpInMemoryCacheFactory>();
            services.AddSingleton<ILpSessionManagerFactory, LpSessionManagerFactory>();
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

        public static IServiceCollection AddLpAppSettingsService<TSettings>(
            this IServiceCollection services,
            string filePath,
            List<JsonConverter> jsonConverters = null)
            where TSettings : class, new()
        {
            services.AddOptions<AppSettingsServiceOptions>().Configure(o =>
            {
                o.FilePath = filePath;
                o.JsonConverters = jsonConverters;
            });
            services.AddSingleton<IAppSettingsService<TSettings>, AppSettingsService<TSettings>>();
            return services;
        }

        public static IServiceCollection AddLpAppLocalizationService(
            this IServiceCollection services,
            List<string> localizationsDirs)
        {
            services.AddOptions<LpLocalizationServiceOptions>().Configure(o =>
            {
                o.LocalizationsDirs = localizationsDirs;
            });
            services.AddSingleton<ILpLocalizationService, LpLocalizationService>();
            return services;
        }
    }
}
