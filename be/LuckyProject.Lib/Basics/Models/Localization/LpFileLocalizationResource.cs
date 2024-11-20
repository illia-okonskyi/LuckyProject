namespace LuckyProject.Lib.Basics.Models.Localization
{
    public class LpFileLocalizationResource : AbstractLpLocalizationResource
    {
        public LpFileLocalizationResource(string source, string locale, string key)
            : base(LpLocalizationResourceType.File, source, locale, key)
        { }

        public string Path { get; set; }
    }
}
