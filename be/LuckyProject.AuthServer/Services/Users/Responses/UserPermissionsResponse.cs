using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class UserPermissionsResponse
    {
        public Guid Id { get; init; }
        public string FullName { get; init; }
        public string Description { get; init; }
        public bool IsSealed { get; init; }
        public Guid FromRoleId { get; init; }
        public string FromRoleName { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LpAuthPermissionType Type { get; private set; }
        public string Name { get; private set; }

        public bool? Allow { get; init; }
        public int? Level { get; init; }
        public HashSet<string> Passkeys { get; init; }


        public void Fullfill(ILpAuthorizationService authService)
        {
            Type = authService.GetPermissionType(FullName);
            Name = authService.GetPermissionName(FullName);
        }
    }
}
