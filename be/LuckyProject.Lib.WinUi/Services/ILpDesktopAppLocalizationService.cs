using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Components.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.Services
{
    public interface ILpDesktopAppLocalizationService : IDisposable
    {
        #region Common
        string CurrentLocale { get; }
        Task SetCurrentLocaleAsync(string locale);

        void AddConsumer(ILpDesktopAppLocalizationConsumer consumer);
        void DeleteConsumer(ILpDesktopAppLocalizationConsumer consumer);
        void OnLocalizationUpdated();

        string GetLocalizedString(string key, string defaultValue = null);
        Dictionary<string, string> GetLocalizedStrings(Dictionary<string, string> keysAndDefaults);
        string GetLocalizedFilePath(string key, string defaultPath = null);
        Dictionary<string, string> GetLocalizedFilePaths(Dictionary<string, string> keysAndDefaults);

        Task LoadSourceAsync(string source);

        LpLocaleInfo GetLocaleInfo(string locale);
        List<LpLocaleInfo> GetDefaultLocales();
        List<LpLocaleInfo> GetAvailableLocales();

        Task<LpLocalizationDocument> LoadDocumentAsync(
            string filePath,
            CancellationToken cancellationToken = default);
        #endregion

        #region Features localizations
        ILpDefaultValidationLocalization ValidationLocalization { get; }
        FsPickerLocalization GetFsPickerControlLocalization();
        PaginationLocalization GetPaginationControlLocalization();
        #endregion
    }
}
