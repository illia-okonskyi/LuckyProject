using LuckyProject.Lib.Basics.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Permissions.Responses
{
    public class PermissionResponse
    {
        public Guid Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LpAuthPermissionType Type { get; init; }
        public string Name { get; init; }
        public string FullName { get; init; }
        public string Description { get; init; }
        public bool IsSealed { get; set; }
        public bool? Allow { get; init; }
        public int? Level { get; init; }
        public HashSet<string> Passkeys { get; init; }
        public Guid? ApiId { get; init; }
    }
}
