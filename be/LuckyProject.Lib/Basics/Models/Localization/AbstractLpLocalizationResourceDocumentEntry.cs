using System;

namespace LuckyProject.Lib.Basics.Models.Localization
{
    public abstract class AbstractLpLocalizationResourceDocumentEntry
    {
        public LpLocalizationResourceType Type { get; }
        public string Key { get; }

        protected AbstractLpLocalizationResourceDocumentEntry(
            LpLocalizationResourceType type,
            string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            Type = type;
            Key = key;
        }
    }
}
