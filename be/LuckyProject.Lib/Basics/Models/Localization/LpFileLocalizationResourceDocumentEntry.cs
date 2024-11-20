namespace LuckyProject.Lib.Basics.Models.Localization
{
    public class LpFileLocalizationResourceDocumentEntry
        : AbstractLpLocalizationResourceDocumentEntry
    {
        public LpFileLocalizationResourceDocumentEntry(string key)
            : base(LpLocalizationResourceType.File, key)
        { }

        public string Path { get; set; }
    }
}
