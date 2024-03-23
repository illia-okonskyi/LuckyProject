using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLpBasicServices(this IServiceCollection services)
        {
            services.AddSingleton<IRuntimeInformationService, RuntimeInformationService>();
            services.AddSingleton<IFsService, FsService>();
            return services;
        }
    }
}
