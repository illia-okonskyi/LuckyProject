using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerBinaryPermissionValue
    {
        public Guid Id { get; set; }
        public Guid PermissionId { get; set; }
        public bool Allow { get; set; }

        public AuthServerPermission Permission { get; set; }
    }
}
