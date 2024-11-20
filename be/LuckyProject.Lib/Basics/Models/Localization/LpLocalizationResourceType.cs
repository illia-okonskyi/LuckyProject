using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LuckyProject.Lib.Basics.Models.Localization
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LpLocalizationResourceType
    {
        String,
        File
    }
}
