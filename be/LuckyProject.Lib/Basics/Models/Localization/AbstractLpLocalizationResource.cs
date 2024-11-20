using System;

namespace LuckyProject.Lib.Basics.Models.Localization
{
    public abstract class AbstractLpLocalizationResource
    {
        public LpLocalizationResourceType Type { get; }
        public string Source { get; }
        public string Locale { get; }
        public string Key { get; }

        protected AbstractLpLocalizationResource(
            LpLocalizationResourceType type,
            string source,
            string locale,
            string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(source);
            ArgumentException.ThrowIfNullOrEmpty(locale);
            ArgumentException.ThrowIfNullOrEmpty(key);
            Type = type;
            Source = source;
            Locale = locale;
            Key = key;
        }
    }
}
