using LuckyProject.Lib.Basics.Models;
using System;

namespace LuckyProject.Lib.Basics.Helpers
{
    public static class AuthorizationHelper
    {
        public static string GetPermissionFullName(LpAuthPermissionType type, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return PermissionTypeToString(type) + name;
        }

        public static string GetPermissionName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || fullName.Length < 2)
            {
                throw new ArgumentException(nameof(fullName));
            }

            return fullName[1..];
        }

        public static LpAuthPermissionType GetPermissionType(string fullName)
        {
            if (string.IsNullOrEmpty(fullName) || fullName.Length < 2)
            {
                throw new ArgumentException(nameof(fullName));
            }

            return PermissionTypeFromChar(fullName[0]);
        }

        public static string PermissionTypeToString(LpAuthPermissionType type)
        {
            return type switch
            {
                LpAuthPermissionType.Root => "r",
                LpAuthPermissionType.Binary => "b",
                LpAuthPermissionType.Level => "l",
                LpAuthPermissionType.Passkey => "p",
                _ => throw new InvalidOperationException("Unexpected Permission type")
            };
        }

        private static LpAuthPermissionType PermissionTypeFromChar(char c)
        {
            return c switch
            {
                'r' => LpAuthPermissionType.Root,
                'b' => LpAuthPermissionType.Binary,
                'l' => LpAuthPermissionType.Level,
                'p' => LpAuthPermissionType.Passkey,
                _ => throw new InvalidOperationException("Unexpected Permission type")
            };
        }
    }
}
