using LuckyProject.Lib.Hosting.EntryPoint;
using NLog.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace LuckyProject.AuthServer
{
    public class EntryPoint : LlAbstractWebHostEntryPoint, IEntryPoint
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
            ConfigureWebServer();
            ConfigureHostedService();
            BuildApp();
            ConfigureApp();
            return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info("Runninng application...");
                await App.RunAsync();
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
                    "${longdate}|${level:format=TriLetter:uppercase=true}|${logger}|${message} ${exception:format=tostring}|url: ${aspnet-request-url}|action: ${aspnet-mvc-action}")
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
            AppBuilder.Logging.ClearProviders();
            AppBuilder.Logging.SetMinimumLevel(LogLevel.Trace);
            AppBuilder.Logging.AddNLog(config);
        }

        private void ConfigureServices()
        {
        }

        private void ConfigureWebServer()
        {
            var cert = new X509Certificate2("config/cert.pfx");
            AppBuilder.WebHost.UseKestrel((context, serverOptions) =>
            {
                serverOptions.ListenAnyIP(5000, listenOptions =>
                {
                    listenOptions.UseHttps(cert);
                });
            });
        }

        private void ConfigureHostedService()
        {
        }

        private void ConfigureApp()
        {
            App.MapGet("/", () => "Hello World!");
        }
        #endregion
    }
}
