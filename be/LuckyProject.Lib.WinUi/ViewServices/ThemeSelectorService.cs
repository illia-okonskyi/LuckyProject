using LuckyProject.Lib.WinUi.Extensions;
using Microsoft.UI.Xaml;

namespace LuckyProject.Lib.WinUi.ViewServices
{
    public class ThemeSelectorService : IThemeSelectorService
    {
        #region Interface impl
        public ElementTheme Theme { get; private set; } = ElementTheme.Default;

        public void Init(ElementTheme theme)
        {
            SetTheme(theme);
        }

        public void SetTheme(ElementTheme theme)
        {
            Theme = theme;
            if (Application.Current.GetLpApplicationRoot()?.MainWindow?.Content
                is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = Theme;
            }
        }
        #endregion
    }
}