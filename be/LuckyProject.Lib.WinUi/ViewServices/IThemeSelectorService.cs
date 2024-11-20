using Microsoft.UI.Xaml;

namespace LuckyProject.Lib.WinUi.ViewServices
{
    public interface IThemeSelectorService
    {
        ElementTheme Theme { get; }
        void Init(ElementTheme theme);
        void SetTheme(ElementTheme theme);
    }
}

