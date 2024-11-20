using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.Lib.WinUi.ViewModels.StatusBar;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using Windows.System;

namespace LuckyProject.Lib.WinUi.Components
{
    public sealed partial class LpShell : Page
    {
        #region VMs & ctor
        public LpShellViewModel ViewModel { get; }
        public LpAppStatusBarViewModel AppStatusBarViewModel { get; }

        public LpShell()
        {
            ViewModel = this.InitViewModel<LpShellViewModel>();
            AppStatusBarViewModel = StaticRootServiceProviderContainer
                .RootServiceProvider
                .GetService<LpAppStatusBarViewModel>();
            InitializeComponent();

            ViewModel.NavigationService.Frame = frmNavigation;
            ViewModel.NavigationViewService.Init(nvMainView);
        }
        #endregion

        #region Internals
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateNavigationViewItems(nvMainView);
            KeyboardAccelerators.Add(BuildKeyboardAccelerator(
                VirtualKey.Left,
                VirtualKeyModifiers.Menu));
            KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
            ViewModel.NavigationService.NavigateToStartingPage(true);
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(
            VirtualKey key,
            VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(
            KeyboardAccelerator sender,
            KeyboardAcceleratorInvokedEventArgs args)
        {
            var navigationService = StaticRootServiceProviderContainer
                .RootServiceProvider
                .GetRequiredService<INavigationService>();
            args.Handled = navigationService.GoBack();
        }
        #endregion
    }
}