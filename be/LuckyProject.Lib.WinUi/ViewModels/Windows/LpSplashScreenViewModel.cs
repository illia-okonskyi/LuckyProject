using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Services;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewModels.Windows
{
    #region Localization
    public partial class LpSplashScreenLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public LpSplashScreenLocalizationViewModel(ILpDesktopAppLocalizationService service)
            : base(service)
        {
            SetProps(GetRegistrations());
        }

        private List<PropRegistration> GetRegistrations()
        {
            var selfType = GetType();
            return new List<PropRegistration>
            {
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.common.strings.version",
                    DefaultValue = "Version:",
                    Property = selfType.GetProperty(nameof(Version))
                }
            };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string version;
        #endregion
    }
    #endregion

    #region Options
    public class LpSplashScreenViewModelOptions
    {
        public Uri ImageUri { get; set; }
        public string AppName { get; set; }
    }
    #endregion

    #region ViewModel
    public partial class LpSplashScreenViewModel
        : LpViewModel<LpSplashScreenLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly IThreadSyncService tsService;

        private object isLoadedLock = new();

        public LpSplashScreenViewModel(
            LpSplashScreenViewModelOptions options,
            IThreadSyncService tsService)
        {
            ImageSource = new BitmapImage(options.ImageUri);
            AppName = options.AppName;
            this.tsService = tsService;
            LoadingTask = Task.Run(
                () => tsService.MonitorSpinWaitAsync(isLoadedLock, () => IsLoaded));
        }
        #endregion

        #region Public & observables
        public bool IsLoaded { get; private set; }
        public Task LoadingTask { get; }
        public void SetLoaded()
        {
            tsService.MonitorGuard(isLoadedLock, () => IsLoaded = true);
        }

        [ObservableProperty]
        private ImageSource imageSource;

        [ObservableProperty]
        private string appName;

        [ObservableProperty]
        private string appVersion;

        [ObservableProperty]
        private LpProgressViewModel progress = new();

        [ObservableProperty]
        private string status;
        #endregion
    }
    #endregion
}
