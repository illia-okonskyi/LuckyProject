using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.DbLayer
{
    public class LpApi
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public string CallbackUrl { get; set; }
        public string MachineClientId { get; set; }
        public Guid MachineUserId { get; set; }

        public AuthServerUser MachineUser { get; set; }
        public ICollection<AuthServerPermission> Permissions { get; set; } =
            new HashSet<AuthServerPermission>();
    }
}
