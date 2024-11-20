using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.Lib.WinUi.ViewModels.Pages;
using System.Collections.Generic;

namespace LuckyProject.LocalizationManager.ViewModels.Pages
{
    #region Localization
    public partial class SettingsPageLocalizationViewModel
        : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public SettingsPageLocalizationViewModel(ILpDesktopAppLocalizationService service)
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
                    Key = "lp.app.lm.pages.settings.strings.description",
                    DefaultValue =
                        "Lucky Project Localization Manager is intented for managing the Lucky " +
                        "Project i18n documents",
                    Property = selfType.GetProperty(nameof(Description))
                },
            };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string description;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class SettingsPageViewModel
        : LpViewModel<SettingsPageLocalizationViewModel>
    {
        #region Internals & ctor
        public SettingsPageViewModel(
            LpSettingsPageBaseViewModel baseViewModel,
            IAppVersionService versionService)
        {
            BaseViewModel = baseViewModel;
            AppVersion = versionService.AppVersion;
            SaveCommand = new(SaveSettingsAsync);
        }
        #endregion

        #region Public & observables
        public LpSettingsPageBaseViewModel BaseViewModel { get; }
        public string AppVersion { get; }
        public RelayCommand SaveCommand { get; }
        #endregion

        #region Internals
        private async void SaveSettingsAsync()
        {
            await BaseViewModel.SaveSettingsAsync();
        }
        #endregion
    }
    #endregion
}