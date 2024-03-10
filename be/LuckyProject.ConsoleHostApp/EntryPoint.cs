using LuckyProject.ConsoleHostApp.Services.Dummy;
using LuckyProject.ConsoleHostApp.Services.Hosted;
using LuckyProject.Lib.Hosting.EntryPoint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp
{
    internal class EntryPoint : LlAbstractGenericHostEntryPoint
    {
        public EntryPoint(string[] args) : base(args)
        { }

        protected override Task ConfigureLoggingAsync()
        {
            // NOTE: Create NLog configuration
            var config = new NLog.Config.LoggingConfiguration();

            var fileTarget = new NLog.Targets.FileTarget("file")
            {
                FileName = "logfile.log",
                Layout = new NLog.Layouts.SimpleLayout(
                    "${longdate}|${level:format=TriLetter:uppercase=true}|${logger}|${message} ${exception:format=tostring}")
            };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);

            var consoleTarget = new NLog.Targets.ColoredConsoleTarget("console")
            {
                Layout = new NLog.Layouts.SimpleLayout(
                    "${level:format=TriLetter:uppercase=true}|${longdate}|${logger}${newline}    ${message} ${exception:format=tostring}")
            };
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, consoleTarget);
            
            // NOTE: Apply NLog configuration
            NLog.LogManager.Configuration = config;

            // NOTE: Configure Logging with NLog
            HostBuilder.Logging.ClearProviders();
            HostBuilder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            HostBuilder.Logging.AddNLog(config);
            return Task.CompletedTask;
        }

        protected override Task ConfigureServicesAsync()
        {
            HostBuilder.Services.AddTransient<IHelloWorldService, HelloWorldService>();
            return Task.CompletedTask;
        }

        protected override Task ConfigureHostedServicesAsync()
        {
            HostBuilder.Services.AddHostedService<LlHostedService>();
            return Task.CompletedTask;
        }
    }
}
