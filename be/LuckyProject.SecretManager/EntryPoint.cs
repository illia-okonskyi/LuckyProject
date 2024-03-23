using LuckyProject.Lib.Hosting.EntryPoint;
using LuckyProject.SecretManager.Models;
using LuckyProject.SecretManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.SecretManager
{
    public class EntryPoint : LlAbstractGenericHostEntryPoint, IEntryPoint
    {
        #region ctor
        public EntryPoint(string[] args) : base(args)
        { }
        #endregion

        #region Public interface
        public override Task ConfigureAsync()
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
                await Host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in application");
            }
        }
        #endregion

        #region Internals
        private void ConfigureLogging()
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
        }

        private void ConfigureServices()
        {
            HostBuilder.Services.Configure<AppConfig>(
                HostBuilder.Configuration.GetSection("Application"));
        }

        private void ConfigureHostedService()
        {
            HostBuilder.Services.AddHostedService<LlSecretManagerService>();
        }
        #endregion
    }
}
