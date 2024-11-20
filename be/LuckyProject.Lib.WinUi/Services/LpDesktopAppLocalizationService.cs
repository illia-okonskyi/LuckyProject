using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Components.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.Services
{
    public class LpDesktopAppLocalizationService<TSettings> : ILpDesktopAppLocalizationService
        where TSettings : class, new()
    {
        #region Internals & ctor & Dispose
        private readonly ILpLocalizationService backend;
        private readonly ILpDesktopAppSettingsService<TSettings> settings;
        private readonly IThreadSyncService tsService;

        private readonly List<string> sources = new();
        private readonly HashSet<ILpDesktopAppLocalizationConsumer> consumers = new();
        private ReaderWriterLockSlim rwlsConsumers = new();

        private readonly LpDefaultValidationLocalization validationLocalization = new();

        public LpDesktopAppLocalizationService(
            ILpLocalizationService backend,
            ILpDesktopAppSettingsService<TSettings> settings,
            IThreadSyncService tsService)
        {
            this.backend = backend;
            this.settings = settings;
            this.tsService = tsService;
        }

        public void Dispose()
        {
            backend.Dispose();
            rwlsConsumers.Dispose();
            rwlsConsumers = null;
        }
        #endregion

        #region Interface impl
        #region Common
        public string CurrentLocale => settings.Locale;

        public async Task SetCurrentLocaleAsync(string locale)
        {
            await backend.SetLocalesAsync(new() { locale });
            settings.Locale = locale;
            OnLocalizationUpdated();
        }

        public void AddConsumer(ILpDesktopAppLocalizationConsumer consumer)
        {
            tsService.ReaderWriterLockSlimWriteGuard(
                rwlsConsumers,
                () => consumers.Add(consumer));
        }

        public void DeleteConsumer(ILpDesktopAppLocalizationConsumer consumer)
        {
            if (rwlsConsumers == null)
            {
                return;
            }

            tsService.ReaderWriterLockSlimWriteGuard(
                rwlsConsumers,
                () => consumers.Remove(consumer));
        }

        public void OnLocalizationUpdated()
        {
            validationLocalization.Update(this);
            var (_, consumers) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsConsumers,
                () => this.consumers.ToList());
            consumers.ForEach(c => c.OnLocalizationUpdated(this));
        }

        public string GetLocalizedString(string key, string defautlvalue = null) =>
            backend.GetLocalizedString(CurrentLocale, key, defautlvalue);

        public Dictionary<string, string> GetLocalizedStrings(
            Dictionary<string, string> keysAndDefaults) =>
            backend.GetLocalizedStrings(CurrentLocale, keysAndDefaults);

        public string GetLocalizedFilePath(string key, string defaultPath = null) =>
            backend.GetLocalizedFilePath(CurrentLocale, key, defaultPath);

        public Dictionary<string, string> GetLocalizedFilePaths(
            Dictionary<string, string> keysAndDefaults) =>
            backend.GetLocalizedFilePaths(CurrentLocale, keysAndDefaults);

        public async Task LoadSourceAsync(string source)
        {
            await backend.LoadSourceAsync(source);
            sources.Add(source);
        }
        public LpLocaleInfo GetLocaleInfo(string locale) => backend.GetLocaleInfo(locale);
        public List<LpLocaleInfo> GetDefaultLocales() => backend.GetDefaultLocales();
        public List<LpLocaleInfo> GetAvailableLocales() => backend.GetAvailableLocales(sources);
        public Task<LpLocalizationDocument> LoadDocumentAsync(
            string filePath,
            CancellationToken cancellationToken = default) =>
            backend.LoadDocumentAsync(filePath, cancellationToken);
        #endregion

        #region Features localization
        public ILpDefaultValidationLocalization ValidationLocalization => validationLocalization;
        public FsPickerLocalization GetFsPickerControlLocalization()
        {
            var defaults = FsPickerLocalization.Default;
            var localization = GetLocalizedStrings(new()
            {
                { "lp.lib.winui.common.strings.select", defaults.Select },
                { "lp.lib.winui.common.strings.dir", defaults.Dir },
                { "lp.lib.winui.common.strings.dirs", defaults.Dirs },
                { "lp.lib.winui.common.strings.file", defaults.File },
                { "lp.lib.winui.common.strings.files", defaults.Files },
            });

            return new FsPickerLocalization
            {
                Select = localization["lp.lib.winui.common.strings.select"],
                Dir = localization["lp.lib.winui.common.strings.dir"],
                Dirs = localization["lp.lib.winui.common.strings.dirs"],
                File = localization["lp.lib.winui.common.strings.file"],
                Files = localization["lp.lib.winui.common.strings.files"],
            };
        }

        public PaginationLocalization GetPaginationControlLocalization()
        {
            var defaults = PaginationLocalization.Default;
            var localization = GetLocalizedStrings(new()
            {
                { "lp.lib.winui.common.strings.showingItems", defaults.ShowingItems },
            });

            return new PaginationLocalization
            {
                ShowingItems = localization["lp.lib.winui.common.strings.showingItems"],
            };
        }
        #endregion
        #endregion
    }
}
