using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Hosting.EntryPoint;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using LuckyProject.LocalizationManager.ViewModels.Pages;
using LuckyProject.LocalizationManager.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager
{
    public class EntryPoint<TApplication>
        : AbstractGenericHostEntryPoint
        , IEntryPoint<TApplication>
        where TApplication : Application, new()
    {
        #region ctor
        public EntryPoint(string[] args)
            : base(args)
        { }
        #endregion

        #region Public interface
        protected override Task ConfigureAsyncImpl()
        {
            ConfigureLogging();
            ConfigureServices();
            ConfigureHostedServices();
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
                logger.Fatal(ex, "Error in application");
            }
        }

        #endregion

        #region Internals
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
            };

            ConfigureLogging_AddFileLogger(config, logLayoutFormat, spamLoggers);
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

        private void ConfigureServices()
        {
            var services = HostBuilder.Services;
            var configuration = HostBuilder.Configuration;

            services.AddLpBasicServices();
            services.AddLpAppVersionService(configuration.GetSection("Application:Version"));
            services.AddLpWinUiServices<
                LmSettings,
                SettingsService,
                SettingsPageViewModel,
                SettingsPageLocalizationViewModel,
                SettingsPage>(
                    "data/app.lm.lp-settings",
                    new() { "i18n" },
                    new() { MaxMessagesCount = 50, CollapsedHeight = 50 });
            services.AddSingleton<ILmProjectService, LmProjectService>();
            services.AddSingleton<ILmDialogService, LmDialogService>();
            services.AddLpViewModel<HomePageViewModel, HomePageLocalizationViewModel>();
            services.AddLpViewModel<
                WorkspacePageViewModel,
                WorkspacePageLocalizationViewModel>(ServiceLifetime.Singleton);
            services.AddTransient<CreateUpdateProjectDialogLocalizationViewModel>();
            services.AddTransient<CreateProjectItemDialogLocalizationViewModel>();
        }

        private void ConfigureHostedServices()
        {
            HostBuilder.Services.AddWinUiHostedService<TApplication>();
        }
        #endregion
    }
}
