using LuckyProject.Lib.Basics.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LuckyProject.Lib.Basics.Models.Localization
{
    public class LpLocalizationDocument
    {
        #region Constants & Statics
        public const string FileExtension = "lpi18n";
        public const string DocType = "lp.doc.i18n";
        public const int DocVersion = 1;
        public static readonly List<JsonConverter> JsonConverters = new()
        {
            new AbstractLpLocalizationResourceDocumentEntryJsonConverter()
        };
        public static readonly Regex FileNameRegex = new(
            @"^(.+)_(.+)\.lpi18n$",
            RegexOptions.Compiled);

        public static string GetFileName(string source, string locale)
        {
            return $"{source}_{locale}.{FileExtension}";
        }

        public static (string Source, string Locale) ParseFileName(string fileName)
        {
            ArgumentException.ThrowIfNullOrEmpty(fileName);
            var match = FileNameRegex.Match(fileName);
            return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : (null, null);
        }
        #endregion

        #region Contents
        public List<AbstractLpLocalizationResourceDocumentEntry> Items { get; set; }
        #endregion
    }
}
