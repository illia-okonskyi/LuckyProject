using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Models;
using Microsoft.UI.Xaml;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.Services
{
    public interface ILpDesktopAppSettingsService
    {
        ElementTheme CurrentTheme { get; set; }
        string Locale { get; set; }
        LpWindowStatePositionSize MainWindowStatePositionSize { get; set; }
        LpAppStatusBarState StatusBarState { get; set; }
        string PickerDialogsSettingsId { get; }

        Task LoadSettingsAsync(CancellationToken cancellationToken = default);
        Task SaveSettingsAsync(CancellationToken cancellationToken = default);
    }

    public interface ILpDesktopAppSettingsService<TSettings>
        : ILpDesktopAppSettingsService
        where TSettings : class, new()
    {
        TSettings CurrentSettings { get; }
    }
}
