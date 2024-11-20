using System;

namespace LuckyProject.AuthServer.Services.Roles.Requests
{
    public class RolePermissionRequest
    {
        public Guid RoleId { get; init; }
        public Guid PermissionId { get; init; }
        public bool IgnoreSealed { get; init; }
    }
}
