using LuckyProject.Lib.Basics.Collections;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.ViewServices.Navigation
{
    public interface INavigationService
    {
        public enum NavItemType
        {
            Item,
            Header
        }

        public class NavItemRegistration : ITreeNode<NavItemRegistration>
        {
            public NavItemType Type { get; set; } = NavItemType.Item;
            public IconElement Icon { get; init; }
            public string TitleLocalizationKey { get; init; }
            public string DefaultTitle { get; init; }
            public IPagesService.PageRegistration Page { get; init; }
            public List<NavItemRegistration> Childs { get; init; }
            public bool IsPage => Page != null;
            public bool IsSettingsPage => IsPage && Page.IsSettingsPage;
        }

        List<NavItemRegistration> Registrations { get; }
        void RegisterNavItems(List<NavItemRegistration> registrations);

        event NavigatedEventHandler Navigated;
        bool CanGoBack { get; }
        Frame Frame { get; set; }
        bool NavigateTo(string pageKey, object parameter = null, bool clearNavigation = false);
        bool NavigateToStartingPage(bool clearNavigation = false);
        bool GoBack();
    }
}

