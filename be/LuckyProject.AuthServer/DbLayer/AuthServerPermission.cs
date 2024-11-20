using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerPermission
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public Guid? ApiId { get; set; } 
        public bool IsSealed { get; set; }

        public AuthServerBinaryPermissionValue BinaryValue { get; set; }
        public AuthServerLevelPermissionValue LevelValue { get; set; }
        public ICollection<AuthServerPasskeyPermissionValue> PasskeyValue { get; set; } =
            new HashSet<AuthServerPasskeyPermissionValue>();
        public LpApi Api { get; set; }
        public ICollection<AuthServerRolePermission> RolePermissions { get; set; } =
            new HashSet<AuthServerRolePermission>();
    }
}
