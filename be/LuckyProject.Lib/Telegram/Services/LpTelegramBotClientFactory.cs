using LuckyProject.Lib.Telegram.Constants;
using LuckyProject.Lib.Telegram.LiveObjects;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace LuckyProject.Lib.Telegram.Services
{
    public class LpTelegramBotClientFactory : ILpTelegramBotClientFactory
    {
        private readonly HttpClient httpClient;
        private readonly ILogger logger;
        
        public LpTelegramBotClientFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<LpTelegramBotClient> logger)
        {
            httpClient = httpClientFactory.CreateClient(
                LpTelegramConstants.Internals.HttpClients.Default);
            this.logger = logger;
        }

        public ILpTelegramBotClient CreateClient(string token, string lpName = null)
        {
            return new LpTelegramBotClient(token, httpClient, lpName, logger);
        }
    }
}
