using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerRolePermission
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public AuthServerRole Role { get; set; }
        public AuthServerPermission Permission { get; set; }
    }
}
