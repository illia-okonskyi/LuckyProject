using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace LuckyProject.Lib.WinUi.AttachedProperties
{
    public static class Navigation
    {
        #region NavigateTo
        public static readonly DependencyProperty NavigateToProperty =
            DependencyProperty.RegisterAttached(
                "NavigateTo",
                typeof(string),
                typeof(Navigation),
                new PropertyMetadata(null));
        public static string GetNavigateTo(NavigationViewItem item) =>
             (string)item.GetValue(NavigateToProperty);
        public static void SetNavigateTo(NavigationViewItem item, string value) =>
            item.SetValue(NavigateToProperty, value);
        #endregion
    }
}
