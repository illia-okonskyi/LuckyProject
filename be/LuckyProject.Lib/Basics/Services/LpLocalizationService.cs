using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Models.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpLocalizationService : ILpLocalizationService
    {
        #region Internals & ctor & Dispose
        private class ResourceKey
        {
            public string Locale { get; init; }
            public string Key { get; init; }

            public override bool Equals(object obj)
            {
                return obj is ResourceKey key &&
                    Locale.Equals(key.Locale, StringComparison.Ordinal) &&
                    Key.Equals(key.Key, StringComparison.Ordinal);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Locale, Key);
            }
        }

        private readonly LpLocalizationServiceOptions options;
        private readonly IThreadSyncService tsService;
        private readonly IFsService fsService;
        private readonly IJsonDocumentService jsonDocumentService;


        private readonly Dictionary<ResourceKey, AbstractLpLocalizationResource> resources = new();
        private readonly HashSet<string> sources = new(StringComparer.Ordinal);
        private HashSet<string> locales = new(StringComparer.Ordinal);
        private readonly ReaderWriterLockSlim rwlsData = new();


        public LpLocalizationService(
            IOptions<LpLocalizationServiceOptions> options,
            IThreadSyncService tsService,
            IFsService fsService,
            IJsonDocumentService jsonDocumentService)
        {
            this.options = options.Value;
            this.tsService = tsService;
            this.fsService = fsService;
            this.jsonDocumentService = jsonDocumentService;
        }

        public void Dispose()
        {
            rwlsData.Dispose();
        }
        #endregion

        #region Interface impl
        public string GetLocalizedString(string locale, string key, string defaultValue = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(locale);
            ArgumentException.ThrowIfNullOrEmpty(key);
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsData,
                () => GetLocalizedStringImpl(locale, key, defaultValue));
            return result;
        }

        public Dictionary<string, string> GetLocalizedStrings(
            string locale,
            Dictionary<string, string> keysAndDefaults)
        {
            ArgumentException.ThrowIfNullOrEmpty(locale);
            ArgumentNullException.ThrowIfNull(keysAndDefaults);
            if (keysAndDefaults.Keys.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException(
                    "Must not have null or empty keys",
                    nameof(keysAndDefaults));
            }

            if (keysAndDefaults.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(rwlsData, () =>
            {
                var r = new Dictionary<string, string>();
                foreach (var kvp in keysAndDefaults)
                {
                    r.Add(kvp.Key, GetLocalizedStringImpl(locale, kvp.Key, kvp.Value));
                }
                return r;
            });

            return result;
        }

        public string GetLocalizedFilePath(string locale, string key, string defaultPath = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(locale);
            ArgumentException.ThrowIfNullOrEmpty(key);
            var ( _, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsData,
                () => GetLocalizedFilePathImpl(locale, key, defaultPath));
            return result;
        }

        public Dictionary<string, string> GetLocalizedFilePaths(
            string locale,
            Dictionary<string, string> keysAndDefaults)
        {
            ArgumentException.ThrowIfNullOrEmpty(locale);
            ArgumentNullException.ThrowIfNull(keysAndDefaults);
            if (keysAndDefaults.Keys.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException(
                    "Must not have null or empty keys",
                    nameof(keysAndDefaults));
            }

            if (keysAndDefaults.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(rwlsData, () =>
            {
                var r = new Dictionary<string, string>();
                foreach (var kvp in keysAndDefaults)
                {
                    r.Add(kvp.Key, GetLocalizedFilePathImpl(locale, kvp.Key, kvp.Value));
                }
                return r;
            });

            return result;
        }

        public async Task SetLocalesAsync(HashSet<string> locales)
        {
            var localesToAdd = locales.Except(this.locales).ToHashSet(this.locales.Comparer);
            var localesToRemove = this.locales.Except(locales).ToHashSet(this.locales.Comparer);

            var (_, task) = tsService.ReaderWriterLockSlimWriteGuard(rwlsData, async () =>
            {
                var keysToRemove = resources
                    .Where(kvp => localesToRemove.Contains(kvp.Value.Locale))
                    .Select(kvp => kvp.Key)
                    .ToList();
                keysToRemove.ForEach(k => resources.Remove(k));
                foreach (var source in sources)
                {
                    await LoadSourceAsync(source, localesToAdd);
                }

                this.locales = locales;
            });
            await task;
        }
    
        public async Task LoadSourceAsync(string source)
        {
            EnsureLocales();
            var (_, task) = tsService.ReaderWriterLockSlimWriteGuard(rwlsData,
                () => LoadSourceAsync(source, locales));
            await task;
        }

        public void UnloadSource(string source)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsData, () =>
            {
                var keysToRemove = resources
                    .Where(kvp => kvp.Value.Source.Equals(source, StringComparison.Ordinal))
                    .Select(kvp => kvp.Key)
                    .ToList();
                keysToRemove.ForEach(k => resources.Remove(k));
                sources.Remove(source);
            });
        }

        public LpLocaleInfo GetLocaleInfo(string locale)
        {
            var cis = CultureInfo.GetCultures(CultureTypes.AllCultures);
            return new LpLocaleInfo
            {
                Name = locale,
                DisplayName = cis
                    .FirstOrDefault(ci => ci.Name.Equals(
                        locale,
                        StringComparison.OrdinalIgnoreCase))
                    ?.DisplayName
                    ?? locale
            };
        }

        public List<LpLocaleInfo> GetDefaultLocales()
        {
            return CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Select(ci => new LpLocaleInfo
                {
                    Name = ci.Name,
                    DisplayName = ci.DisplayName,
                })
                .ToList();
        }

        public List<LpLocaleInfo> GetAvailableLocales(List<string> sources)
        {
            ArgumentNullException.ThrowIfNull(sources);
            if (sources.Count == 0 || sources.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentException(
                    "Must not be empty or have empty or null sources",
                    nameof(sources));
            }

            var localesBySources = options.LocalizationsDirs
                .SelectMany(d => fsService
                    .DirectoryEnumerateFiles(d, $"*.{LpLocalizationDocument.FileExtension}")
                    .Select(p => LpLocalizationDocument
                        .ParseFileName(fsService.PathGetFileName(p))))
                .Where(p => sources.Contains(p.Source))
                .Distinct()
                .GroupBy(p => p.Source)
                .Select(p => new
                {
                    Source = p.Key,
                    Locales = p.Select(p1 => p1.Locale).ToHashSet()
                })
                .ToList();
            if (localesBySources.Count == 0)
            {
                return [];
            }

            var locales = localesBySources[0].Locales;
            for (int i = 1; i < localesBySources.Count; ++i)
            {
                locales = locales.Intersect(localesBySources[i].Locales).ToHashSet();
            }

            var cis = CultureInfo.GetCultures(CultureTypes.AllCultures);
            return locales
                .Select(l => new LpLocaleInfo
                {
                    Name = l,
                    DisplayName = cis
                        .FirstOrDefault(ci => ci.Name.Equals(l, StringComparison.OrdinalIgnoreCase))
                        ?.DisplayName
                        ?? l
                })
                .ToList();
        }

        public async Task<LpLocalizationDocument> LoadDocumentAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var doc = await jsonDocumentService
                    .ReadDocumentFromFileAsync<LpLocalizationDocument>(
                        filePath,
                        LpLocalizationDocument.DocType,
                        LpLocalizationDocument.DocVersion,
                        null,
                        LpLocalizationDocument.JsonConverters);
                return doc.Document;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Internals
        private string GetLocalizedStringImpl(string locale, string key, string defaultValue)
        {
            if (!resources.TryGetValue(new() { Locale = locale, Key = key }, out var resource))
            {
                return defaultValue;
            }

            if (resource.Type != LpLocalizationResourceType.String)
            {
                return defaultValue;
            }

            return ((LpStringLocalizationResource)resource).Value;
        }

        private string GetLocalizedFilePathImpl(string locale, string key, string defaultPath)
        {
            if (!resources.TryGetValue(new() { Locale = locale, Key = key }, out var resource))
            {
                return defaultPath;
            }

            if (resource.Type != LpLocalizationResourceType.File)
            {
                return defaultPath;
            }

            return ((LpFileLocalizationResource)resource).Path;
        }

        private async Task LoadSourceAsync(string source, HashSet<string> locales)
        {
            foreach (var locale in locales)
            {
                await LoadSourceAsync(source, locale);
            }
            sources.Add(source);
        }

        private async Task LoadSourceAsync(string source, string locale)
        {
            var doc = await LoadDocAsync(LpLocalizationDocument.GetFileName(source, locale));
            if (doc == null)
            {
                return;
            }

            AddSource(source, locale, doc.Items);
        }

        private async Task<LpLocalizationDocument> LoadDocAsync(string fileName)
        {
            foreach (var dir in options.LocalizationsDirs)
            {
                var filePath = fsService.PathCombine(dir, fileName);
                var doc = await LoadDocumentAsync(filePath);
                if (doc != null)
                {
                    return doc;
                }
            }

            return null;
        }

        private void AddSource(
            string source,
            string locale,
            List<AbstractLpLocalizationResourceDocumentEntry> items)
        {
            var entries = items.Select(i =>
            {
                AbstractLpLocalizationResource entry = i.Type switch
                {
                    LpLocalizationResourceType.String =>
                        new LpStringLocalizationResource(source, locale, i.Key)
                        {
                            Value = ((LpStringLocalizationResourceDocumentEntry)i).Value
                        },

                    LpLocalizationResourceType.File =>
                        new LpFileLocalizationResource(source, locale, i.Key)
                        {
                            Path = ((LpFileLocalizationResourceDocumentEntry)i).Path,
                        },
                    _ => throw new NotImplementedException()
                };
                return entry;
            }).ToList();
            entries.ForEach(e => resources.Add(new() { Locale = locale, Key = e.Key}, e));
        }

        private void EnsureLocales()
        {
            if (locales.Count == 0)
            {
                throw new InvalidOperationException("Locales not set");
            }
        }
        #endregion
    }
}
