using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.Lib.WinUi.ViewModels.Windows;
using LuckyProject.Lib.WinUi.ViewModels.Pages;
using LuckyProject.Lib.WinUi.ViewServices;
using LuckyProject.Lib.WinUi.ViewServices.Activation;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using Newtonsoft.Json;
using LuckyProject.Lib.WinUi.ViewModels.StatusBar;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using LuckyProject.Lib.WinUi.EntryPoint;

namespace LuckyProject.Lib.WinUi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region AddLpWinUiServices
        public static IServiceCollection AddLpWinUiServices<
            TSettings,
            TDesktopAppSettingsService,
            TSettingsPageViewModel,
            TSettingsPageLocalizationViewModel,
            TSettingsView>(
                this IServiceCollection services,
                string settingsFilePath,
                List<string> localizationsDirs,
                LpAppStatusBarOptions statusBarOptions,
                List<JsonConverter> settingsJsonConverters = null)
            where TSettings : class, new()
            where TDesktopAppSettingsService : class, ILpDesktopAppSettingsService<TSettings>
            where TSettingsPageViewModel : LpViewModel<TSettingsPageLocalizationViewModel>
            where TSettingsPageLocalizationViewModel : AbstractLpLocalizationViewModel
            where TSettingsView : Page
        {
            // NOTE: Settings services
            services.AddLpAppSettingsService<TSettings>(settingsFilePath, settingsJsonConverters);
            services.AddSingleton<ILpDesktopAppSettingsService, TDesktopAppSettingsService>();
            services.AddSingleton<
                ILpDesktopAppSettingsService<TSettings>,
                TDesktopAppSettingsService>();

            // NOTE: Localization services
            services.AddLpAppLocalizationService(localizationsDirs);
            services.AddSingleton<
                ILpDesktopAppLocalizationService,
                LpDesktopAppLocalizationService<TSettings>>();

            // NOTE: View Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<
                INavigationViewService,
                NavigationViewService<TSettingsPageViewModel>>();
            services.AddSingleton<IActivationService, ActivationService<TSettings>>();
            services.AddSingleton<IPagesService, PagesService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ILpDialogService, LpDialogService>();
            services.AddSingleton<ITaskbarService, TaskbarService>();

            // NOTE: View Models
            services.AddLpViewModel<
                TSettingsPageViewModel,
                TSettingsPageLocalizationViewModel>();
            services.AddLpViewModel<
                LpSettingsPageBaseViewModel,
                LpSettingsPageBaseLocalizationViewModel>();
            services.AddOptions<LpAppStatusBarOptions>().Configure(o =>
            {
                o.MaxMessagesCount = statusBarOptions.MaxMessagesCount;
                o.CollapsedHeight = statusBarOptions.CollapsedHeight;
            });
            services.AddLpViewModel<
                LpAppStatusBarViewModel,
                LpAppStatusBarLocalizationViewModel>(ServiceLifetime.Singleton);
            services.AddSingleton<ILpAppStatusBarService, LpAppStatusBarViewModel>(
                sp => sp.GetRequiredService<LpAppStatusBarViewModel>());
            services.AddTransient<LpShellViewModel>();
            services.AddTransient<LpSplashScreenLocalizationViewModel>();
            services.AddTransient<LpValidationViewModel>();
            services.AddTransient<LpPaginationLocalizationViewModel>();
            return services;
        }

        public static IServiceCollection AddWinUiHostedService<TApplication>(
            this IServiceCollection services)
            where TApplication : Application, new()
        {
            services.AddHostedService<WinUiHostedService<TApplication>>();
            return services;
        }
        #endregion

        #region AddLpLocalizeable VMs
        public static IServiceCollection AddLpViewModel<TViewModel, TLocalizationViewModel>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TViewModel : LpViewModel<TLocalizationViewModel>
            where TLocalizationViewModel : AbstractLpLocalizationViewModel
        {
            services.Add(new(
                typeof(TLocalizationViewModel),
                typeof(TLocalizationViewModel),
                lifetime));
            services.Add(new(
                typeof(TViewModel),
                typeof(TViewModel),
                lifetime));
            return services;
        }

        public static IServiceCollection AddLpRecipientViewModel<TViewModel, TLocalizationViewModel>(
            this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            where TViewModel : LpRecipientViewModel<TLocalizationViewModel>
            where TLocalizationViewModel : AbstractLpLocalizationViewModel
        {
            services.Add(new(
                typeof(TLocalizationViewModel),
                typeof(TLocalizationViewModel),
                lifetime));
            services.Add(new(
                typeof(TViewModel),
                typeof(TViewModel),
                lifetime));
            return services;
        }
        #endregion
    }
}
