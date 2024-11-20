using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.WinUi.Extensions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace LuckyProject.Lib.WinUi.ViewServices.Navigation
{
    public class NavigationService : INavigationService
    {
        #region Internal & ctor
        private readonly IPagesService pagesService;
        private object lastParameterUsed;
        private Frame frame;

        public NavigationService(IPagesService pagesService)
        {
            this.pagesService = pagesService;
        }
        #endregion

        #region Interface impl
        public List<INavigationService.NavItemRegistration> Registrations { get; private set; }

        public void RegisterNavItems(List<INavigationService.NavItemRegistration> registrations)
        {
            Registrations = registrations;
            var pages = new List<IPagesService.PageRegistration>();
            registrations.ForEach(r => pages
                .AddRange(r
                    .EnumerateInWidth()
                    .Where(pair => pair.Item.IsPage)
                    .Select(pair => pair.Item.Page)));
            pagesService.RegisterPages(pages);
        }

        public event NavigatedEventHandler Navigated;
        public Frame Frame
        {
            get
            {
                if (frame == null)
                {
                    frame = Application.Current.GetLpApplicationRoot().MainWindow.Content as Frame;
                    RegisterFrameEvents();
                }

                return frame;
            }

            set
            {
                UnregisterFrameEvents();
                frame = value;
                RegisterFrameEvents();
            }
        }

        [MemberNotNullWhen(true, nameof(Frame), nameof(frame))]
        public bool CanGoBack => Frame?.CanGoBack ?? false;
        public bool GoBack()
        {
            if (!CanGoBack)
            {
                return false;
            }

            var vmBeforeNavigation = frame.GetPageViewModel();
            frame.GoBack();
            if (vmBeforeNavigation is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedFrom();
            }

            return true;
        }

        public bool NavigateTo(
            string pageKey,
            object parameter = null,
            bool clearNavigation = false)
        {
            var pageType = pagesService.GetPageType(pageKey);
            if (pageType == null)
            {
                return false;
            }

            var canNavigateTo = frame != null &&
                (
                    frame.Content?.GetType() != pageType ||
                    (
                        parameter != null &&
                        !parameter.Equals(lastParameterUsed
                    )
                ));
            if (!canNavigateTo)
            {
                return false;
            }

            frame.Tag = clearNavigation;
            var vmBeforeNavigation = frame.GetPageViewModel();
            var navigated = frame.Navigate(pageType, parameter);
            if (navigated)
            {
                lastParameterUsed = parameter;
                if (vmBeforeNavigation is INavigationAware navigationAware)
                {
                    navigationAware.OnNavigatedFrom();
                }
            }

            return navigated;
        }

        public bool NavigateToStartingPage(bool clearNavigation = false)
        {
            return NavigateTo(
                pagesService.GetPages().First(p => p.IsStartingPage).Key,
                clearNavigation: clearNavigation);
        }
        #endregion

        #region Internals
        private void RegisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated += OnNavigated;
            }
        }

        private void UnregisterFrameEvents()
        {
            if (frame != null)
            {
                frame.Navigated -= OnNavigated;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is not Frame frame)
            {
                return;
            }

            var clearNavigation = (bool)frame.Tag;
            if (clearNavigation)
            {
                frame.BackStack.Clear();
            }

            if (frame.GetPageViewModel() is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(e.Parameter);
            }

            Navigated?.Invoke(sender, e);
        }
        #endregion
    }
}