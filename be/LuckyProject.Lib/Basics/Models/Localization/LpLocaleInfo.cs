namespace LuckyProject.Lib.Basics.Models.Localization
{
    public record LpLocaleInfo 
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
