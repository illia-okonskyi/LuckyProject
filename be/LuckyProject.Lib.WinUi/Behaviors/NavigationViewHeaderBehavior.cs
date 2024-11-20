using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Xaml.Interactivity;

namespace LuckyProject.Lib.WinUi.Behaviors
{
    public class NavigationViewHeaderBehavior : Behavior<NavigationView>
    {
        #region Internals
        private static NavigationViewHeaderBehavior current;
        private Page currentPage;
        #endregion
        
        #region Public properties
        public DataTemplate DefaultHeaderTemplate { get; set; }
        #endregion

        #region DependancyProperties
        #region DefaultHeader
        public static readonly DependencyProperty DefaultHeaderProperty =
            DependencyProperty.Register(
                nameof(DefaultHeader),
                typeof(object),
                typeof(NavigationViewHeaderBehavior),
                new PropertyMetadata(null, (d, e) => current!.UpdateHeader()));
        public object DefaultHeader
        {
            get => GetValue(DefaultHeaderProperty);
            set => SetValue(DefaultHeaderProperty, value);
        }
        #endregion
        #endregion

        #region AttachedProperties
        #region HeaderMode
        public static readonly DependencyProperty HeaderModeProperty =
            DependencyProperty.RegisterAttached(
                "HeaderMode",
                typeof(bool),
                typeof(NavigationViewHeaderBehavior),
                new PropertyMetadata(
                    NavigationViewHeaderMode.Always,
                    (d, e) => current!.UpdateHeader()));
        public static NavigationViewHeaderMode GetHeaderMode(Page item) =>
            (NavigationViewHeaderMode)item.GetValue(HeaderModeProperty);
        public static void SetHeaderMode(Page item, NavigationViewHeaderMode value) =>
            item.SetValue(HeaderModeProperty, value);
        #endregion

        #region HeaderContext
        public static readonly DependencyProperty HeaderContextProperty =
            DependencyProperty.RegisterAttached(
                "HeaderContext",
                typeof(object),
                typeof(NavigationViewHeaderBehavior),
                new PropertyMetadata(null, (d, e) => current!.UpdateHeader()));
        public static object GetHeaderContext(Page item) => item.GetValue(HeaderContextProperty);
        public static void SetHeaderContext(Page item, object value) =>
            item.SetValue(HeaderContextProperty, value);
        #endregion

        #region HeaderTemplate
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.RegisterAttached(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(NavigationViewHeaderBehavior),
                new PropertyMetadata(null, (d, e) => current!.UpdateHeaderTemplate()));
        public static DataTemplate GetHeaderTemplate(Page item) =>
            (DataTemplate)item.GetValue(HeaderTemplateProperty);
        public static void SetHeaderTemplate(Page item, DataTemplate value) =>
            item.SetValue(HeaderTemplateProperty, value);
        #endregion
        #endregion

        #region Internals
        protected override void OnAttached()
        {
            base.OnAttached();

            var navigationService = StaticRootServiceProviderContainer
                .RootServiceProvider
                .GetRequiredService<INavigationService>();
            navigationService.Navigated += OnNavigated;

            current = this;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            var navigationService = StaticRootServiceProviderContainer
                .RootServiceProvider
                .GetRequiredService<INavigationService>();
            navigationService.Navigated -= OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (sender is Frame frame && frame.Content is Page page)
            {
                currentPage = page;

                UpdateHeader();
                UpdateHeaderTemplate();
            }
        }

        private void UpdateHeader()
        {
            if (currentPage == null)
            {
                return;
            }

            var headerMode = GetHeaderMode(currentPage);
            if (headerMode == NavigationViewHeaderMode.Never)
            {
                AssociatedObject.Header = null;
                AssociatedObject.AlwaysShowHeader = false;
                return;
            }

            AssociatedObject.Header = GetHeaderContext(currentPage) ?? DefaultHeader;
            AssociatedObject.AlwaysShowHeader = (headerMode == NavigationViewHeaderMode.Always);
        }

        private void UpdateHeaderTemplate()
        {
            if (currentPage != null)
            {
                var headerTemplate = GetHeaderTemplate(currentPage);
                AssociatedObject.HeaderTemplate = headerTemplate ?? DefaultHeaderTemplate;
            }
        }
        #endregion
    }
}