using LuckyProject.Lib.WinUi.Components;
using LuckyProject.Lib.WinUi.ViewServices;
using LuckyProject.Lib.WinUi.ViewModels.Windows;
using LuckyProject.Lib.WinUi.Services;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public interface ILpApplicationRoot
    {
        IServiceProvider ServiceProvider { get; }
        LpMainWindow MainWindow { get; }
        LpShell Shell { get; }
        ILpAppStatusBarService AppStatusBarService { get; }
        bool CanClose { get; set; }
        bool HasUnsavedData { get; set; }

        bool TryEnqueueToDispatcherQueue(DispatcherQueueHandler callback);
        bool TryEnqueueToDispatcherQueue(
            DispatcherQueuePriority priority,
            DispatcherQueueHandler callback);
        void ExitApp(int exitCode = 0, bool force = false);
        int ExitCode { get; }
    }

    public interface ILpApplicationRoot<TSettings> : ILpApplicationRoot
            where TSettings : class, new()
    {
        Task OnLaunched(
            LaunchActivatedEventArgs args,
            LpSplashScreenViewModelOptions splashScreenOptions,
            Func<
                LpApplicationInitContext<TSettings>,
                Task<List<INavigationService.NavItemRegistration>>> initAsync,
            Action<LpMainWindow> mainWindowSetup = null);
    }
}