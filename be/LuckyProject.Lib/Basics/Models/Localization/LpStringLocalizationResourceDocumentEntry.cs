namespace LuckyProject.Lib.Basics.Models.Localization
{
    public class LpStringLocalizationResourceDocumentEntry
        : AbstractLpLocalizationResourceDocumentEntry
    {
        public LpStringLocalizationResourceDocumentEntry(string key)
            : base(LpLocalizationResourceType.String, key)
        { }

        public string Value { get; set; }
    }
}
