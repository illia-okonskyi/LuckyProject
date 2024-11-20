using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.AttachedProperties
{
    public static class Pages
    {
        #region HeaderExtraContent
        public static readonly DependencyProperty HeaderExtraContentProperty =
            DependencyProperty.RegisterAttached(
                "HeaderExtraContent",
                typeof(object),
                typeof(Pages),
                new PropertyMetadata(null));
        public static object GetHeaderExtraContent(Page page) =>
            page.GetValue(HeaderExtraContentProperty);
        public static void SetHeaderExtraContent(Page page, object value) =>
            page.SetValue(HeaderExtraContentProperty, value);
        #endregion
    }
}
