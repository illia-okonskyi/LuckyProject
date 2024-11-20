namespace LuckyProject.Lib.Basics.Models.Localization
{
    public class LpStringLocalizationResource : AbstractLpLocalizationResource
    {
        public LpStringLocalizationResource(string source, string locale, string key)
            : base(LpLocalizationResourceType.String, source, locale, key)
        { }

        public string Value { get; set; }
    }
}
