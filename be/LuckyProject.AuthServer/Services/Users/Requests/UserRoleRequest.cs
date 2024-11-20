using System;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class UserRoleRequest
    {
        public Guid UserId { get; init; }
        public Guid RoleId { get; init; }
        public bool IgnoreSealed { get; init; }
    }
}
