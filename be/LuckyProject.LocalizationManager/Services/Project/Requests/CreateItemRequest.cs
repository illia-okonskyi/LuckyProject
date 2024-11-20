using LuckyProject.Lib.Basics.Models.Localization;

namespace LuckyProject.LocalizationManager.Services.Project.Requests
{
    public class CreateItemRequest
    {
        public LpLocalizationResourceType ResourceType { get; init; }
        public string Key { get; init; }
    }
}
