using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

namespace LuckyProject.Lib.WinUi.Extensions
{
    public static class UiExtensions
    {
        #region InitViewModel
        public static TViewModel InitViewModel<TViewModel>(
            this Control control,
            TViewModel vm)
            where TViewModel : ObservableObject
        {
            control.DataContext = vm;
            return vm;
        }

        public static TViewModel InitViewModel<TViewModel>(this Control control)
            where TViewModel : ObservableObject
        {
            var vm = StaticRootServiceProviderContainer
                .RootServiceProvider
                .GetRequiredService<TViewModel>();
            control.InitViewModel(vm);
            return vm;
        }
        #endregion
    }
}
