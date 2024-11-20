using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerPasskeyPermissionValue
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
        public string Passkey { get; set; }

        public AuthServerPermission Permission { get; set; }
    }
}
