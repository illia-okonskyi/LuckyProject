using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Web.Helpers
{
    public static class LpJwtHelper
    {
        public static List<LpAuthRequirement> ParseAuthRequirements(string s)
        {
            return s.Split(
                ';',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(ParseAuthRequirement)
                .ToList();
        }

        #region Internals
        private static readonly Regex PermissionFullNameRegex = new(
            @"^[a-zA-Z0-9\-_]{2,64}$",
            RegexOptions.Compiled);
        private static readonly Regex PermissionPasskeyRegex = new(
            @"^[a-zA-Z0-9\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\'\""\<\>\/\?]{16,64}$",
            RegexOptions.Compiled);
        private static LpAuthRequirement ParseAuthRequirement(string s)
        {
            var colonIndex = s.IndexOf(':');
            var permissionFullName = (colonIndex == -1) ? s : s[0..colonIndex];

            if (!PermissionFullNameRegex.IsMatch(permissionFullName))
            {
                throw new ArgumentException(
                    $"{permissionFullName} requirement name invalid",
                    nameof(s));
            }

            var permissionType = AuthorizationHelper.GetPermissionType(permissionFullName);

            var mustHaveParameter = permissionType == LpAuthPermissionType.Level ||
                permissionType == LpAuthPermissionType.Passkey;
            if (!mustHaveParameter)
            {
                return new LpAuthRequirement { PermissionFullName = permissionFullName };
            }

            if (colonIndex == -1)
            {
                throw new ArgumentException(
                    $"{permissionFullName} requirement must have parameter",
                    nameof(s));
            }

            var parameter = s[(colonIndex + 1)..];
            if (permissionType == LpAuthPermissionType.Level)
            {
                if (!int.TryParse(parameter, CultureInfo.InvariantCulture, out var expectedLevel))
                {
                    throw new ArgumentException(
                        $"{permissionFullName} requirement must be int",
                        nameof(s));
                }

                if (expectedLevel < 2)
                {
                    throw new ArgumentException(
                        $"{permissionFullName} requirement expected level must be > 1",
                        nameof(s));
                }

                return new LpAuthRequirement
                {
                    PermissionFullName = permissionFullName,
                    ExpectedValue = expectedLevel
                };
            }

            var expectedPasskeys = parameter.Split(
                '|',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToHashSet();
            if (expectedPasskeys.Count == 0)
            {
                throw new ArgumentException(
                    $"{permissionFullName} requirement must have expected passkeys",
                    nameof(s));
            }

            foreach (var passkey in expectedPasskeys)
            {
                if (!PermissionPasskeyRegex.IsMatch(passkey))
                {
                    throw new ArgumentException(
                        $"{permissionFullName} requirement expected passkey {passkey} invalid",
                        nameof(s));
                }
            }

            return new LpAuthRequirement
            {
                PermissionFullName = permissionFullName,
                ExpectedValue = expectedPasskeys
            };
        }
        #endregion
    }
}
