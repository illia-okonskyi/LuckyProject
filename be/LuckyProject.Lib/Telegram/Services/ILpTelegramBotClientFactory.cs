using LuckyProject.Lib.Telegram.LiveObjects;

namespace LuckyProject.Lib.Telegram.Services
{
    public interface ILpTelegramBotClientFactory
    {
        ILpTelegramBotClient CreateClient(string token, string lpName = null);
    }
}
