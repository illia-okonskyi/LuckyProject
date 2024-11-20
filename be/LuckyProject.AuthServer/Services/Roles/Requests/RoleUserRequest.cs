using System;

namespace LuckyProject.AuthServer.Services.Roles.Requests
{
    public class RoleUserRequest
    {
        public Guid RoleId { get; init; }
        public Guid UserId { get; init; }
        public bool IgnoreSealed { get; init; }
    }
}
