using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.LocalizationManager.Models;
using Microsoft.UI.Xaml;
using System;

namespace LuckyProject.LocalizationManager.Services
{
    public class SettingsService : AbstractLpDesktopAppSettingsService<LmSettings>
    {
        public SettingsService(IAppSettingsService<LmSettings> backend)
            : base(backend)
        { }

        public override ElementTheme CurrentTheme
        {
            get => CurrentSettings.Theme;
            set => CurrentSettings.Theme = value;
        }
        public override string Locale
        {
            get => CurrentSettings.Locale;
            set => CurrentSettings.Locale = value;
        }
        public override LpWindowStatePositionSize MainWindowStatePositionSize
        {
            get => CurrentSettings.MainWindowStatePostionSize;
            set => CurrentSettings.MainWindowStatePostionSize = value;
        }
        public override LpAppStatusBarState StatusBarState
        {
            get => CurrentSettings.StatusBarState;
            set => CurrentSettings.StatusBarState = value;
        }
        public override string PickerDialogsSettingsId => CurrentSettings.PickerDialogsSettingsId;
    }
}
