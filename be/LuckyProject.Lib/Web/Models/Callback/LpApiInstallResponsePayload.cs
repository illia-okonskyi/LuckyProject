using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.Lib.Web.Models.Callback
{
    public class LpApiInstallResponsePayload
    {
        public class Permission
        {
            public LpAuthPermissionType Type { get; init; }
            public string Name { get; init; }
            public string Description { get; init; }
            public bool? Allow { get; init; }
            public int? Level { get; init; }
            public HashSet<string> Passkeys { get; init; }
        }

        public string Name { get; init; }
        public string Description { get; init; }
        public List<string> Origins { get; init; } = new();
        public string Endpoint { get; init; }
        public string MachineUserEmail { get; init; }
        public string MachineUserPhoneNumber { get; init; }
        public string MachineUserPrefferedLocale { get; init; }
        public List<Permission> Permissions { get; init; } = new();
    }
}
