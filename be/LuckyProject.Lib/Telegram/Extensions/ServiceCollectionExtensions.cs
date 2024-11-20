using LuckyProject.Lib.Telegram.Constants;
using LuckyProject.Lib.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LuckyProject.Lib.Telegram.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLpTelegramServices(this IServiceCollection services)
        {
            services.AddHttpClient(LpTelegramConstants.Internals.HttpClients.Default);
            services.AddSingleton<ILpTelegramBotClientFactory, LpTelegramBotClientFactory>();
            return services;
        }
    }
}
