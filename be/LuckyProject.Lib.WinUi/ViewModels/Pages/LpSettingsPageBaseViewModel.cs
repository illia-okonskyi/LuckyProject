using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewServices;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewModels.Pages
{
    #region Localization
    public partial class LpSettingsPageBaseLocalizationViewModel
        : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public LpSettingsPageBaseLocalizationViewModel(ILpDesktopAppLocalizationService service)
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
                    Key = "lp.lib.winui.pages.settings.strings.personalization",
                    DefaultValue = "Personalization",
                    Property = selfType.GetProperty(nameof(Personalization))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.theme",
                    DefaultValue = "Theme",
                    Property = selfType.GetProperty(nameof(Theme))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.themeLight",
                    DefaultValue = "Light",
                    Property = selfType.GetProperty(nameof(ThemeLight))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.themeDark",
                    DefaultValue = "Dark",
                    Property = selfType.GetProperty(nameof(ThemeDark))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.themeDefault",
                    DefaultValue = "Default",
                    Property = selfType.GetProperty(nameof(ThemeDefault))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.localization",
                    DefaultValue = "Localization",
                    Property = selfType.GetProperty(nameof(Localization))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.language",
                    DefaultValue = "Language",
                    Property = selfType.GetProperty(nameof(Language))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.pages.settings.strings.about",
                    DefaultValue = "About this application",
                    Property = selfType.GetProperty(nameof(About))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.common.strings.version",
                    DefaultValue = "Version:",
                    Property = selfType.GetProperty(nameof(Version))
                },
            };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string personalization;

        [ObservableProperty]
        private string theme;

        [ObservableProperty]
        private string themeLight;

        [ObservableProperty]
        private string themeDark;

        [ObservableProperty]
        private string themeDefault;

        [ObservableProperty]
        private string localization;

        [ObservableProperty]
        private string language;

        [ObservableProperty]
        private string about;

        [ObservableProperty]
        private string version;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class LpSettingsPageBaseViewModel
        : LpViewModel<LpSettingsPageBaseLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly ILpDesktopAppSettingsService settingsService;
        private readonly IThemeSelectorService themeSelectorService;

        public LpSettingsPageBaseViewModel(
            ILpDesktopAppSettingsService settingsService,
            IThemeSelectorService themeSelectorService,
            ILpDesktopAppLocalizationService localizationService)
        {
            this.settingsService = settingsService;
            this.themeSelectorService = themeSelectorService;

            theme = this.settingsService.CurrentTheme;
            Locales = Localization.Service.GetAvailableLocales();
            var settingsLocale = localizationService.GetLocaleInfo(settingsService.Locale);
            Locale = Locales.Find(l => l == settingsLocale) ?? Locales.FirstOrDefault();
            themeSelectorService.SetTheme(Theme);
        }
        #endregion

        #region Public & observables
        [ObservableProperty]
        private ElementTheme theme;

        public List<LpLocaleInfo> Locales { get; }
        [ObservableProperty]
        private LpLocaleInfo locale;
        #endregion

        #region Public methods
        public async Task SaveSettingsAsync()
        {
            settingsService.CurrentTheme = Theme;
            settingsService.Locale = Locale?.Name;
            await settingsService.SaveSettingsAsync();
            themeSelectorService.SetTheme(Theme);
            await Localization.Service.SetCurrentLocaleAsync(Locale?.Name);
        }
        #endregion
    }
    #endregion
}
