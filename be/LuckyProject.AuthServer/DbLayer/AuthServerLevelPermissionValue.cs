using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerLevelPermissionValue
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
        public int Level { get; set; }

        public AuthServerPermission Permission { get; set; }
    }
}
