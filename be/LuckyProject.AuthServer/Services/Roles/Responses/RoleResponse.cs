using System;

namespace LuckyProject.AuthServer.Services.Roles.Responses
{
    public class RoleResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string NormalizedName { get; init; }
        public string Description { get; init; }
        public bool IsSealed { get; set; }
    }
}
