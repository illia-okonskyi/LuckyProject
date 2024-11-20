using LuckyProject.Lib.Basics.Models.Localization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpLocalizationService : IDisposable
    {
        string GetLocalizedString(string locale, string key, string defaultValue = null);
        Dictionary<string, string> GetLocalizedStrings(
            string locale,
            Dictionary<string, string> keysAndDefaults);
        string GetLocalizedFilePath(string locale, string key, string defaultPath = null);
        Dictionary<string, string> GetLocalizedFilePaths(
            string locale,
            Dictionary<string, string> keysAndDefaults);

        Task SetLocalesAsync(HashSet<string> locales);
        Task LoadSourceAsync(string source);
        void UnloadSource(string source);

        LpLocaleInfo GetLocaleInfo(string locale);
        List<LpLocaleInfo> GetDefaultLocales();
        List<LpLocaleInfo> GetAvailableLocales(List<string> sources);

        Task<LpLocalizationDocument> LoadDocumentAsync(
            string filePath,
            CancellationToken cancellationToken = default);
    }
}
