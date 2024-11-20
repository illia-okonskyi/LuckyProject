using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Models;
using Microsoft.UI.Xaml;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.Services
{
    public abstract class AbstractLpDesktopAppSettingsService<TSettings>
        : ILpDesktopAppSettingsService<TSettings>
        where TSettings : class, new()
    {
        #region Internals & ctor & Dispose
        private readonly IAppSettingsService<TSettings> backend;

        public AbstractLpDesktopAppSettingsService(IAppSettingsService<TSettings> backend)
        {
            this.backend = backend;
        }

        public void Dispose() => backend.Dispose();
        #endregion

        #region Backend fallback
        public TSettings CurrentSettings => backend.CurrentSettings;

        public Task LoadSettingsAsync(CancellationToken cancellationToken = default) =>
            backend.LoadAsync(cancellationToken);
        public Task SaveSettingsAsync(CancellationToken cancellationToken = default) =>
            backend.SaveAsync(cancellationToken);
        #endregion

        #region Abstract interface
        public abstract ElementTheme CurrentTheme { get; set; }
        public abstract string Locale { get; set; }
        public abstract LpWindowStatePositionSize MainWindowStatePositionSize { get; set; }
        public abstract LpAppStatusBarState StatusBarState { get; set; }
        public abstract string PickerDialogsSettingsId { get; }
        #endregion
    }
}
