using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.Components;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewServices;
using LuckyProject.Lib.WinUi.ViewServices.Activation;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using LuckyProject.Lib.WinUi.Components.Windows;
using LuckyProject.Lib.WinUi.ViewModels.Windows;
using LuckyProject.Lib.WinUi.ViewModels.StatusBar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using WinUIEx;
using Microsoft.UI.Dispatching;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public class LpApplicationRoot<TSettings> : ILpApplicationRoot<TSettings>
        where TSettings : class, new()
    {
        #region Internals & ctor
        private readonly LpApplicationCloseOptions closeOptions;
        private ILpDesktopAppSettingsService<TSettings> settingsService;
        private ILpDesktopAppLocalizationService localizationService;
        private LpAppStatusBarViewModel statusBarVm;
        private ILpDialogService dialogService;
        private int requestedExitCode;

        public LpApplicationRoot(LpApplicationCloseOptions closeOptions = null)
        {
            ServiceProvider = StaticRootServiceProviderContainer.RootServiceProvider;
            this.closeOptions = closeOptions ?? new();
        }
        #endregion

        #region Interface impl
        public IServiceProvider ServiceProvider { get; }
        public LpMainWindow MainWindow { get; private set; }
        public LpShell Shell { get; private set; }
        public ILpAppStatusBarService AppStatusBarService => statusBarVm;
        public bool CanClose { get; set; } = true;
        public bool HasUnsavedData { get; set; }
        public bool TryEnqueueToDispatcherQueue(DispatcherQueueHandler callback) =>
            MainWindow.DispatcherQueue.TryEnqueue(callback);
        public bool TryEnqueueToDispatcherQueue(
            DispatcherQueuePriority priority,
            DispatcherQueueHandler callback) =>
            MainWindow.DispatcherQueue.TryEnqueue(priority, callback);

        public void ExitApp(int exitCode = 0, bool force = false)
        {
            if (force)
            {
                ExitCode = exitCode;
                MainWindow ??= new LpMainWindow(OnMainWindowClosingAsync);
                MainWindow.ForceClose();
                return;
            }
            else
            {
                requestedExitCode = exitCode;
                MainWindow.Close();
            }
        }

        public int ExitCode { get; private set; }

        public async Task OnLaunched(
            LaunchActivatedEventArgs args,
            LpSplashScreenViewModelOptions splashScreenOptions,
            Func<
                LpApplicationInitContext<TSettings>,
                Task<List<INavigationService.NavItemRegistration>>> initAsync,
            Action<LpMainWindow> mainWindowSetup = null)
        {
            var serviceProvider = StaticRootServiceProviderContainer.RootServiceProvider;
            var logger = serviceProvider
                .GetRequiredService<ILogger<LpApplicationRoot<TSettings>>>();

            LpSplashScreenViewModel splashScreenVm = null;
            try
            {
                logger.LogInformation("Starting app...");
                var tsService = serviceProvider.GetRequiredService<IThreadSyncService>();
                splashScreenVm = new LpSplashScreenViewModel(
                    splashScreenOptions,
                    tsService);
                var splashScreen = new LpSplashScreen(splashScreenVm, new WindowEx());

                var appVersionService = serviceProvider.GetRequiredService<IAppVersionService>();
                settingsService = serviceProvider
                    .GetRequiredService<ILpDesktopAppSettingsService<TSettings>>();
                localizationService = serviceProvider
                    .GetRequiredService<ILpDesktopAppLocalizationService>();

                var navigationService = serviceProvider.GetRequiredService<INavigationService>();
                var activationService = serviceProvider.GetRequiredService<IActivationService>();
                var themeSelectorService = serviceProvider
                    .GetRequiredService<IThemeSelectorService>();
                dialogService = serviceProvider.GetRequiredService<ILpDialogService>();

                await OnLaunched_InitBaseServicesAsync(splashScreenVm, appVersionService);
                var navItems = await initAsync(CreateInitContext(splashScreenVm));
                OnLaunched_InitViewServices(
                    navigationService,
                    themeSelectorService,
                    navItems);
                OnLaunched_InitView(serviceProvider, mainWindowSetup);
                await OnLaunched_ActivateAsync(activationService, args);
                logger.LogInformation("Started at {UtcNow:o} UTC", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Init failed");
                ExitApp(ExitCodes.Unknown, true);
            }
            finally
            {
                splashScreenVm?.SetLoaded();
                splashScreenVm?.Dispose();
            }
        }

        private async Task OnLaunched_InitBaseServicesAsync(
            LpSplashScreenViewModel splashScreenVm,
            IAppVersionService appVersionService)
        {
            splashScreenVm.Progress.IsIndeterminate = true;
            splashScreenVm.Status = "Loading application version...";
            await appVersionService.InitAsync();
            splashScreenVm.AppVersion = appVersionService.AppVersion;

            splashScreenVm.Status = "Loading application settings...";
            await settingsService.LoadSettingsAsync();

            splashScreenVm.Status = "Loading Lucky Project WinUi Library localizations...";
            await localizationService.SetCurrentLocaleAsync(settingsService.Locale);
            await localizationService.LoadSourceAsync("lp-lib-winui");
            localizationService.OnLocalizationUpdated();
        }

        private void OnLaunched_InitViewServices(
            INavigationService navigationService,
            IThemeSelectorService themeSelectorService,
            List<INavigationService.NavItemRegistration> navItems)
        {
            navigationService.RegisterNavItems(navItems);
            themeSelectorService.Init(settingsService.CurrentTheme);
        }

        private void OnLaunched_InitView(
            IServiceProvider serviceProvider,
            Action<LpMainWindow> mainWindowSetup)
        {
            Shell = new();
            MainWindow = new(OnMainWindowClosingAsync)
            {
                Content = Shell,
            };
            statusBarVm = serviceProvider.GetRequiredService<LpAppStatusBarViewModel>();
            var statusBarState = settingsService.StatusBarState ?? LpAppStatusBarState.Default;
            statusBarVm.IsExpanded = statusBarState.IsExpanded;
            statusBarVm.SavedExpandedHeight = statusBarState.SavedExpandedHeight;

            mainWindowSetup?.Invoke(MainWindow);
        }

        private async Task OnLaunched_ActivateAsync(
            IActivationService activationService,
            LaunchActivatedEventArgs args)
        {
            await activationService.ActivateAsync(
                args,
                () => MainWindow.Content ??= Shell,
                () => MainWindow.Activate(settingsService));
        }
        #endregion

        #region Internals
        private LpApplicationInitContext<TSettings> CreateInitContext(
            LpSplashScreenViewModel splashScreenVm)
        {
            var loading = localizationService.GetLocalizedString(
                "lp.lib.winui.common.strings.loading",
                "Loading...");
            var loading1 = localizationService.GetLocalizedString(
                "lp.lib.winui.common.strings.loading1",
                "Loading {0}...");
            var loading2 = localizationService.GetLocalizedString(
                "lp.lib.winui.common.strings.loading2",
                "Loading {0} {1}...");
            var localization = localizationService.GetLocalizedString(
                "lp.lib.winui.common.strings.localization",
                "Localization");

            return new LpApplicationInitContext<TSettings>
            {
                Localizations = new()
                {
                    Loading = loading,
                    Loading1 = loading1,
                    Loading2 = loading2,

                    GetLocalizedString = localizationService.GetLocalizedString,
                    GetLocalizedStrings = localizationService.GetLocalizedStrings,
                    GetLocalizedFilePath = localizationService.GetLocalizedFilePath,
                    GetLocalizedFilePaths = localizationService.GetLocalizedFilePaths
                },
                SetInitProgressValue = v => splashScreenVm.Progress.Value = v,
                SetInitProgressIsIndeterminate = i => splashScreenVm.Progress.IsIndeterminate = i,
                SetInitStatus = s => splashScreenVm.Status = s,
                LoadLocalizationSourceAsync = async source =>
                {
                    var status = string.Format(loading2, localization, source);
                    splashScreenVm.Status = status;
                    await localizationService.LoadSourceAsync(source);
                },
                CurrentSettings = settingsService.CurrentSettings
            };
        }

        private async Task<bool> OnMainWindowClosingAsync()
        {
            var verifyResult = await OnMainWindowClosingAsync_VerifyPreconditions();
            if (verifyResult == OnMainWindowClosingAction.None)
            {
                return false;
            }

            if (verifyResult == OnMainWindowClosingAction.SaveAndClose)
            {
                await OnMainWindowClosingAsync_SaveApplicationData();
            }

            if (closeOptions.ExtraCloseAsyncHandler != null &&
                !await closeOptions.ExtraCloseAsyncHandler())
            {
                return false;
            }

            await OnMainWindowClosingAsync_SaveInternalData();
            ExitCode = requestedExitCode;
            return true;
        }

        private enum OnMainWindowClosingAction
        {
            None,
            SaveAndClose,
            Close
        }

        private async Task<OnMainWindowClosingAction> OnMainWindowClosingAsync_VerifyPreconditions()
        {
            if (!CanClose)
            {
                return OnMainWindowClosingAction.None;
            }

            if (AppStatusBarService.IsBusy)
            {
                var localization = localizationService.GetLocalizedStrings(new()
                {
                    {
                        "lp.lib.winui.entrypoint.strings.closeConfirmation",
                        "Close confirmation"
                    },
                    {
                        "lp.lib.winui.entrypoint.strings.busyQuestion",
                        "There are some pending tasks still in progess.\r\nDo you want to close " +
                        "the application?"
                    }
                });
                var busyConfirmation = await dialogService.ShowMessageBoxAsync(new()
                {
                    Title = localization["lp.lib.winui.entrypoint.strings.closeConfirmation"],
                    Text = localization["lp.lib.winui.entrypoint.strings.busyQuestion"],
                    Icon = LpMessageBoxIcon.Question,
                    Buttons = LpMessageBoxButtons.YesNo
                });
                if (busyConfirmation == LpMessageBoxResult.No)
                {
                    return OnMainWindowClosingAction.None;
                }
            }

            if (HasUnsavedData && closeOptions.SaveAppDataAsyncHandler != null)
            {
                var localization = localizationService.GetLocalizedStrings(new()
                {
                    {
                        "lp.lib.winui.entrypoint.strings.closeConfirmation",
                        "Close confirmation"
                    },
                    {
                        "lp.lib.winui.entrypoint.strings.unsavedDataQuestion",
                        "There are unsaved data in the application.\r\nDo you want to save data " +
                        "before close the application?"
                    }
                });
                var unsavedConfirmation = await dialogService.ShowMessageBoxAsync(new()
                {
                    Title = localization["lp.lib.winui.entrypoint.strings.closeConfirmation"],
                    Text = localization["lp.lib.winui.entrypoint.strings.unsavedDataQuestion"],
                    Icon = LpMessageBoxIcon.Question,
                    Buttons = LpMessageBoxButtons.YesNoCancel
                });
                if (unsavedConfirmation == LpMessageBoxResult.Cancel)
                {
                    return OnMainWindowClosingAction.None;
                }

                if (unsavedConfirmation == LpMessageBoxResult.Yes)
                {
                    return OnMainWindowClosingAction.SaveAndClose;
                }

                return OnMainWindowClosingAction.Close;
            }

            return OnMainWindowClosingAction.Close;
        }

        private async Task OnMainWindowClosingAsync_SaveApplicationData()
        {
            var title = localizationService.GetLocalizedString(
                "lp.lib.winui.entrypoint.strings.savingDataMessage",
                "Saving application data...");
            await dialogService.ShowProgressDialogAsync(new()
            {
                Title = title,
                IsIndeterminate = true,
                Handler = closeOptions.SaveAppDataAsyncHandler
            });
        }

        private async Task OnMainWindowClosingAsync_SaveInternalData()
        {
            if (closeOptions.SaveMainWindowState)
            {
                var mwState = ((OverlappedPresenter)MainWindow.Presenter).State switch
                {
                    OverlappedPresenterState.Maximized => LpWindowState.Maximized,
                    OverlappedPresenterState.Minimized => LpWindowState.Minimized,
                    _ => LpWindowState.Normal,
                };
                var mwSettings = settingsService.MainWindowStatePositionSize ?? new();
                mwSettings.State = mwState;
                if (mwState == LpWindowState.Normal)
                {
                    var position = MainWindow.AppWindow.Position;
                    var size = MainWindow.AppWindow.Size;
                    mwSettings.X = position.X;
                    mwSettings.Y = position.Y;
                    mwSettings.Width = size.Width;
                    mwSettings.Height = size.Height;
                }
                settingsService.MainWindowStatePositionSize = mwSettings;
            }

            if (closeOptions.SaveStatusBarState)
            {
                var sbIsExpanded = statusBarVm.IsExpanded;
                statusBarVm.IsExpanded = false;
                settingsService.StatusBarState = new()
                {
                    IsExpanded = sbIsExpanded,
                    SavedExpandedHeight = statusBarVm.SavedExpandedHeight
                };
            }

            if (closeOptions.SaveMainWindowState || closeOptions.SaveStatusBarState)
            {
                await settingsService.SaveSettingsAsync();
            }
        }
        #endregion
    }
}

