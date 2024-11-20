using LuckyProject.Lib.Basics.JsonConverters;
using LuckyProject.Lib.Basics.Models.Localization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Policy;

namespace LuckyProject.LocalizationManager.Models
{
    public class LmProjectDocument
    {
        #region Constants
        public const string FileExtension = "lp-lm-proj";
        public const string DocType = "lp.doc.app.lm.proj";
        public const int DocVersion = 1;
        public static readonly List<JsonConverter> JsonConverters = new()
        {
            new AbstractLpLocalizationResourceDocumentEntryJsonConverter()
        };
        public static readonly List<string> SelectFileTypeFilter = [$".{FileExtension}"];
        public static readonly Dictionary<string, List<string>> SaveFileTypeChoises = new()
        {
            { "LP LM Project", SelectFileTypeFilter }
        };
        public static readonly List<string> BaselineSelectFileTypeFilter =
            [$".{LpLocalizationDocument.FileExtension}"];
        #endregion

        #region Entries
        public enum ItemStatus
        {
            New,
            Review,
            Done
        }

        public class Item
        {
            public ItemStatus Status { get; set; } = ItemStatus.New;
            public LpLocalizationResourceType ResourceType { get; init; }
                = LpLocalizationResourceType.String;
            public string Key { get; init; }
            public string Basis { get; set; }
            public string Transaltion { get; set; }
        }
        #endregion

        #region Document
        public string Name { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Locale { get; set; }
        public List<Item> Items { get; set; } = new();
        #endregion
    }
}
