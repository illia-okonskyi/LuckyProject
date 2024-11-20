using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerRole : IdentityRole<Guid>
    {
        public string Description { get; set; }
        public bool IsSealed { get; set; }
        public ICollection<AuthServerRolePermission> RolePermissions { get; set; } =
            new HashSet<AuthServerRolePermission>();
    }
}
