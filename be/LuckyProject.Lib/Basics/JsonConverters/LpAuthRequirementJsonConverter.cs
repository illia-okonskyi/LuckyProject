using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.JsonConverters
{
    public class LpAuthRequirementJsonConverter : JsonConverter<LpAuthRequirement>
    {
        public override LpAuthRequirement ReadJson(
            JsonReader reader,
            Type objectType,
            LpAuthRequirement existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var permissionFullName = jObject[nameof(LpAuthRequirement.PermissionFullName)]
                    .Value<string>();
                var type = AuthorizationHelper.GetPermissionType(permissionFullName);
                var expectedValueProperty = jObject[nameof(LpAuthRequirement.ExpectedValue)];
                object expectedValue = type switch
                {
                    LpAuthPermissionType.Root => null,
                    LpAuthPermissionType.Binary => expectedValueProperty.Value<bool>(),
                    LpAuthPermissionType.Level => expectedValueProperty.Value<int>(),
                    LpAuthPermissionType.Passkey => expectedValueProperty.Value<HashSet<string>>(),
                    _ => null
                };

                return new LpAuthRequirement
                {
                    PermissionFullName = permissionFullName,
                    ExpectedValue = expectedValue
                };
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override void WriteJson(
            JsonWriter writer,
            LpAuthRequirement value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
