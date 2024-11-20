using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.WinUi.AttachedProperties;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewServices;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    public partial class LpShellViewModel
        : ObservableRecipient
        , ILpDesktopAppLocalizationConsumer
    {
        #region Internals & ctor & Dispose
        private class NavItemEntry
        {
            public INavigationService.NavItemRegistration Registration { get; init; }
            public NavigationViewItem NavigationViewItem { get; init; }
            public NavigationViewItemHeader NavigationViewItemHeader { get; init; }
            public PageHeaderViewModel HeaderViewModel { get; init; }
        }
        
        private readonly IPagesService pagesService;
        private readonly ILpDesktopAppLocalizationService localizationService;
        private readonly List<NavItemEntry> navItems = new();

        public LpShellViewModel(
            IPagesService pagesService,
            INavigationService navigationService,
            INavigationViewService navigationViewService,
            ILpDesktopAppLocalizationService localizationService)
        {
            this.pagesService = pagesService;
            this.localizationService = localizationService;
            NavigationService = navigationService;
            NavigationService.Navigated += OnNavigated;
            NavigationViewService = navigationViewService;
            this.localizationService.AddConsumer(this);
        }

        public void Dispose()
        {
            localizationService.DeleteConsumer(this);
        }
        #endregion

        #region Public & observables
        public INavigationService NavigationService { get; }
        public INavigationViewService NavigationViewService { get; }
        public void UpdateNavigationViewItems(NavigationView nv)
        {
            var registrations = NavigationService.Registrations;
            var settingsPage = new NavItemEntry
            {
                Registration = registrations.First(r => r.IsPage && r.Page.IsSettingsPage),
                NavigationViewItem = (NavigationViewItem)NavigationViewService.SettingsItem,
                HeaderViewModel = new()
            };
            navItems.Add(settingsPage);
            foreach (var registration in registrations)
            {
                if (registration.IsSettingsPage)
                {
                    continue;
                }

                foreach (var (parent, item) in registration.EnumerateInWidth())
                {
                    NavigationViewItemHeader navHeader = null;
                    NavigationViewItem navItem = null;
                    NavigationViewItemBase navItemBase = null;
                    if (item.Type == INavigationService.NavItemType.Header)
                    {
                        navHeader = new NavigationViewItemHeader();
                        navItemBase = navHeader;
                    }
                    else
                    {
                        navItem = new NavigationViewItem()
                        {
                            Icon = item.Icon ?? new FontIcon
                            {
                                FontFamily = (FontFamily)Application
                                    .Current
                                    .Resources["SymbolThemeFontFamily"],
                                Glyph = "\uE729"
                            },
                            SelectsOnInvoked = false,
                        };

                        if (item.IsPage)
                        {
                            navItem.SelectsOnInvoked = true;
                            Navigation.SetNavigateTo(navItem, item.Page.Key);
                        }

                        navItemBase = navItem;
                    }

                    if (parent == null)
                    {
                        nv.MenuItems.Add(navItemBase);
                    }

                    var parentNavItem = navItems
                        .FirstOrDefault(e => e.Registration == parent)
                        ?.NavigationViewItem;
                    if (parentNavItem != null)
                    {
                        parentNavItem.MenuItems.Add(navItemBase);
                    }

                    navItems.Add(new NavItemEntry
                    {
                        Registration = item,
                        NavigationViewItem = navItem,
                        NavigationViewItemHeader = navHeader,
                        HeaderViewModel = navItem != null ? new() : null
                    });
                }
            }

            UpdateNavItems();
        }

        [ObservableProperty]
        private bool isBackEnabled;
        [ObservableProperty]
        private object selectedNavItem;
        [ObservableProperty]
        private PageHeaderViewModel selectedHeaderViewModel;
        #endregion

        #region Internals
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;

            if (e.SourcePageType == pagesService.GetSettingsPageType())
            {
                UpdateSelected(NavigationViewService.SettingsItem, e.Content as Page);
                return;
            }

            var selectedNavItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
            if (selectedNavItem != null)
            {
                UpdateSelected(selectedNavItem, e.Content as Page);
            }
        }

        private void UpdateSelected(object navItem, Page view)
        {
            var page = navItems.FirstOrDefault(p =>
                p.Registration.IsPage && p.Registration.Page.ViewType == view.GetType());
            if (page == null)
            {
                return;
            }

            var extraContent = AttachedProperties.Pages.GetHeaderExtraContent(view);
            if (extraContent != null)
            {
                page.HeaderViewModel.ExtraContent = extraContent;
            }
            SelectedHeaderViewModel = page.HeaderViewModel;
            SelectedNavItem = navItem;
        }

        public void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            UpdateNavItems();
        }

        private void UpdateNavItems()
        {
            var keysAndDefaults = navItems.ToDictionary(
                p => p.Registration.TitleLocalizationKey,
                p => p.Registration.DefaultTitle);
            var localization = localizationService.GetLocalizedStrings(keysAndDefaults);
            foreach (var navItem in navItems)
            {
                var title = localization[navItem.Registration.TitleLocalizationKey];
                if (navItem.Registration.Type == INavigationService.NavItemType.Item)
                {
                    navItem.NavigationViewItem.Content = title;
                    navItem.HeaderViewModel.Title = title;
                }
                else
                {
                    navItem.NavigationViewItemHeader.Content = title;
                }
            }
        }
        #endregion
    }
}
