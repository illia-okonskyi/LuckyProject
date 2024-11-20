using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.ViewServices.Navigation
{
    public class NavigationViewService<TSettingsViewModel> : INavigationViewService
        where TSettingsViewModel : ObservableObject
    {
        #region Internals & ctor
        private readonly INavigationService navigationService;
        private readonly IPagesService pagesService;
        private NavigationView navigationView;

        public NavigationViewService(
            INavigationService navigationService,
            IPagesService pagesService)
        {
            this.navigationService = navigationService;
            this.pagesService = pagesService;
        }
        #endregion

        #region Interface impl
        public IList<object> MenuItems => navigationView?.MenuItems;
        public object SettingsItem => navigationView?.SettingsItem;

        [MemberNotNull(nameof(NavigationViewService<TSettingsViewModel>.navigationView))]
        public void Init(NavigationView navigationView)
        {
            this.navigationView = navigationView;
            this.navigationView.BackRequested += OnBackRequested;
            this.navigationView.ItemInvoked += OnItemInvoked;
        }

        public void UnregisterEvents()
        {
            if (navigationView != null)
            {
                navigationView.BackRequested -= OnBackRequested;
                navigationView.ItemInvoked -= OnItemInvoked;
            }
        }

        public NavigationViewItem GetSelectedItem(Type pageType)
        {
            if (navigationView == null)
            {
                return null;
            }

            var selectedItem = GetSelectedItem(
                navigationView.MenuItems,
                pageType);
            return selectedItem ?? GetSelectedItem(navigationView.FooterMenuItems, pageType);
        }
        #endregion

        #region Internals
        private void OnBackRequested(
            NavigationView sender,
            NavigationViewBackRequestedEventArgs args) => navigationService.GoBack();

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                navigationService.NavigateTo(typeof(TSettingsViewModel).FullName);
            }
            else
            {
                var selectedItem = args.InvokedItemContainer as NavigationViewItem;
                if (selectedItem?.GetValue(AttachedProperties.Navigation.NavigateToProperty)
                    is string pageKey)
                {
                    navigationService.NavigateTo(pageKey);
                }
            }
        }

        private NavigationViewItem GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            if (menuItem.GetValue(AttachedProperties.Navigation.NavigateToProperty)
                is string pageKey)
            {
                return pagesService.GetPageType(pageKey) == sourcePageType;
            }

            return false;
        }
        #endregion
    }
}
