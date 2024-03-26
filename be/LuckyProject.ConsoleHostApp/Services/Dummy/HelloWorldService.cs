using Microsoft.Extensions.Logging;

namespace LuckyProject.ConsoleHostApp.Services.Dummy
{
    public class HelloWorldService : IHelloWorldService
    {
        private readonly ILogger logger;

        public HelloWorldService(ILogger<HelloWorldService> logger)
        {
            this.logger = logger;
        }

        public void SayHello(string from)
        {
            logger.LogWarning($"Hello from {from}");
        }
    }
}
