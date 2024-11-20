using LuckyProject.Lib.WinUi.Models;
using Microsoft.UI.Xaml;

namespace LuckyProject.LocalizationManager.Models
{
    public class LmSettings
    {
        public ElementTheme Theme { get; set; } = ElementTheme.Default;
        public string Locale { get; set; } = "en-us";
        public LpWindowStatePositionSize MainWindowStatePostionSize { get; set; } = new()
        {
            State = LpWindowState.Normal,
            Width = 1500,
            Height = 875
        };

        public LpAppStatusBarState StatusBarState { get; set; } = new()
        {
            IsExpanded = LpAppStatusBarState.Default.IsExpanded,
            SavedExpandedHeight = LpAppStatusBarState.Default.SavedExpandedHeight
        };

        public string PickerDialogsSettingsId => "lp.app.lm";
    }
}
