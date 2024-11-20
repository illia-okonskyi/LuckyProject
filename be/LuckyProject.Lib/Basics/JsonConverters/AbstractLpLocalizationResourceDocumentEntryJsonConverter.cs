using LuckyProject.Lib.Basics.Models.Localization;
using Newtonsoft.Json.Linq;
using System;

namespace LuckyProject.Lib.Basics.JsonConverters
{
    public class AbstractLpLocalizationResourceDocumentEntryJsonConverter
        : AbstractJsonCreationConverter<AbstractLpLocalizationResourceDocumentEntry>
    {
        protected override AbstractLpLocalizationResourceDocumentEntry Create(
            Type objectType,
            JObject jObject)
        {
            var typeString = jObject[nameof(AbstractLpLocalizationResourceDocumentEntry.Type)]
                .Value<string>();
            if (!Enum.TryParse(typeString, out LpLocalizationResourceType type))
            {
                return null;
            }
            var key = jObject[nameof(AbstractLpLocalizationResourceDocumentEntry.Key)]
                .Value<string>();

            return type switch
            {
                LpLocalizationResourceType.String =>
                    new LpStringLocalizationResourceDocumentEntry(key),
                LpLocalizationResourceType.File =>
                    new LpFileLocalizationResourceDocumentEntry(key),
                _ => throw new NotImplementedException()
            };
        }
    }
}