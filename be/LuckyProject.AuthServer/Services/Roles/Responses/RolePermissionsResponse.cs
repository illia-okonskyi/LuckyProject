using System;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LuckyProject.AuthServer.Services.Roles.Responses
{
    public class RolePermissionsResponse
    {
        public Guid Id { get; init; }
        public string FullName { get; init; }
        public string Description { get; init; }
        public bool IsSealed { get; init; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LpAuthPermissionType Type { get; private set; }
        public string Name { get; private set; }

        public void Fullfill(ILpAuthorizationService authService)
        {
            Type = authService.GetPermissionType(FullName);
            Name = authService.GetPermissionName(FullName);
        }
    }
}
