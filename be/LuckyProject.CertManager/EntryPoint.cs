using LuckyProject.CertManager.Services;
using LuckyProject.Lib.Azure.Extensions;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Hosting.EntryPoint;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Config;
using NLog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.CertManager
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
            ConfigureConfiguration();
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
        private void ConfigureConfiguration()
        {
            HostBuilder.Configuration.AddJsonFile("appsecret.json");
        }

        private void ConfigureLogging()
        {
            // NOTE: Create NLog configuration
            var config = new NLog.Config.LoggingConfiguration();

            var logLayoutFormat = "${level:format=TriLetter:uppercase=true}|${longdate}|" +
                "${logger}|${scopenested:separator=>}${newline}    ${scopeindent}${message} " +
                "${exception:format=tostring}";
            var msSpamLogger = "Microsoft.Hosting.*";

            var fileTarget = new NLog.Targets.FileTarget("file")
            {
                FileName = "logfile.log",
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            //config.AddRule(new LoggingRule(msSpamLogger, NLog.LogLevel.Off, fileTarget)
            //{
            //    FinalMinLevel = NLog.LogLevel.Off
            //});
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget);

            var consoleTarget = new NLog.Targets.ColoredConsoleTarget("console")
            {
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            //config.AddRule(new LoggingRule(msSpamLogger, NLog.LogLevel.Off, consoleTarget)
            //{
            //    FinalMinLevel = NLog.LogLevel.Off
            //});
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
            var services = HostBuilder.Services;
            var configuration = HostBuilder.Configuration;

            services.AddLpBasicServices();
            services.AddLpAppVersionService(configuration.GetSection("Application:Version"));
            services.AddLpAzureBasicServices();
            services.AddLpAzureAppRegistrationService(
                configuration.GetSection("Application:AzureAppRegistration"));
            services.Configure<LpCertManagerServiceOptions>(
                configuration.GetSection("Application:Service"));
        }

        private void ConfigureHostedService()
        {
            HostBuilder.Services.AddHostedService<LpCertManagerService>();
        }
        #endregion
    }
}
