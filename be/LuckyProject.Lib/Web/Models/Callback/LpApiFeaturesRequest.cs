using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Web.Models.Callback
{
    public class LpApiFeaturesRequest
    {
        public class Permission
        {
            public LpAuthPermissionType Type { get; init; }
            public string Name { get; init; }
            public bool? Allow { get; init; }
            public int? Level { get; init; }
            public HashSet<string> Passkeys { get; init; }
        }

        public List<Permission> Permissions { get; init; } = new();

        public bool HasRootPermission()
        {
            return Permissions.Any(p => p.Type == LpAuthPermissionType.Root);
        }

        public bool HasBinaryPermission(string name)
        {
            return Permissions.Any(
                p => p.Type == LpAuthPermissionType.Binary &&
                p.Name == name &&
                p.Allow == true);
        }

        public bool HasLevelPermission(string name, int expectedLevel)
        {
            return Permissions.Any(
                p => p.Type == LpAuthPermissionType.Level &&
                p.Name == name &&
                p.Level.Value >= expectedLevel);
        }

        public bool HasPasskeyPermission(string name, string passkey)
        {
            return Permissions.Any(
                p => p.Type == LpAuthPermissionType.Passkey &&
                p.Name == name &&
                p.Passkeys.Contains(passkey));
        }
    }
}
