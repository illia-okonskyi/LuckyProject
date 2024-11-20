using LuckyProject.ConsoleHostApp.Services.Dummy;
using LuckyProject.ConsoleHostApp.Services.Hosted;
using LuckyProject.Lib.Azure.Extensions;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Hosting.EntryPoint;
using LuckyProject.Lib.Telegram.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp
{
    public class EntryPoint : AbstractGenericHostEntryPoint, IEntryPoint
    {
        #region ctor
        public EntryPoint(string[] args) : base(args)
        { }
        #endregion

        #region Public interface
        protected override Task ConfigureAsyncImpl()
        {
            ConfigureLogging();
            ConfigureServices();
            ConfigureHostedService();
            BuildHost();
            return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info("Runninng application...");
                await Host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in application");
            }
        }
        #endregion

        #region Internals
        #region ConfigureLogging
        private void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logLayoutFormat =
                "${level:format=TriLetter:uppercase=true}|${longdate}|${logger}${newline}" +
                "    [${scopenested:separator=>}]${newline}" +
                "    ${scopeindent}${message} ${exception:format=tostring}";
            var spamLoggers = new List<string>
            {
                "Microsoft.Hosting.*",
                "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware",
                "Quartz.*"
            };

            ConfigureLogging_AddFileLogger(config, logLayoutFormat, spamLoggers);
            ConfigureLogging_AddConsoleLogger(config, logLayoutFormat, spamLoggers);
            ConfigureLogging_Finish(config);
        }

        private void ConfigureLogging_AddFileLogger(
            LoggingConfiguration config,
            string logLayoutFormat,
            List<string> spamLoggers)
        {
            var target = new NLog.Targets.FileTarget("file")
            {
                FileName = "logfile.log",
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            ConfigureLogging_SwallowSpamLoggers(config, spamLoggers, target);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);
        }

        private void ConfigureLogging_AddConsoleLogger(
            LoggingConfiguration config,
            string logLayoutFormat,
            List<string> spamLoggers)
        {
            var target = new NLog.Targets.ColoredConsoleTarget("console")
            {
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            ConfigureLogging_SwallowSpamLoggers(config, spamLoggers, target);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);
        }

        private void ConfigureLogging_SwallowSpamLoggers(
            LoggingConfiguration config,
            List<string> spamLoggers,
            Target target)
        {
            foreach (var spamLogger in spamLoggers)
            {
                config.AddRule(new LoggingRule(spamLogger, NLog.LogLevel.Off, target)
                {
                    FinalMinLevel = NLog.LogLevel.Off
                });
            }
        }
        private void ConfigureLogging_Finish(LoggingConfiguration config)
        {
            NLog.LogManager.Configuration = config;
            HostBuilder.Logging.ClearProviders();
            HostBuilder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            HostBuilder.Logging.AddNLog(config);
        }
        #endregion

        private void ConfigureServices()
        {
            HostBuilder.Services.AddLpBasicServices();
            HostBuilder.Services.AddLpTelegramServices();
            HostBuilder.Services.AddLpAzureCsService(HostBuilder.Configuration.GetSection("AzureCs"));



            //HostBuilder.Services.AddScoped<IHelloWorldService, HelloWorldService>();
        }

        private void ConfigureHostedService()
        {
            //HostBuilder.Services.AddHostedService<LpSingleRunPrimaryHostedService>();
            //HostBuilder.Services.AddHostedService<LpPeriodicBackgroundHostedService>();
            HostBuilder.Services.AddHostedService<LpTestTelegramBotService>();
        }
        #endregion
    }
}
