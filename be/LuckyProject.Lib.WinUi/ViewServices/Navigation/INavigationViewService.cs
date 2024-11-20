using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.ViewServices.Navigation
{
    public interface INavigationViewService
    {
        IList<object> MenuItems { get; }
        object SettingsItem { get; }
        void Init(NavigationView navigationView);
        void UnregisterEvents();
        NavigationViewItem GetSelectedItem(Type pageType);
    }
}

