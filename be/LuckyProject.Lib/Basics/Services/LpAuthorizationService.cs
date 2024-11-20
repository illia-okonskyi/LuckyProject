using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpAuthorizationService : ILpAuthorizationService
    {
        #region Name/type operations
        public string GetPermissionTypePrefix(LpAuthPermissionType type) =>
            AuthorizationHelper.PermissionTypeToString(type);
        public string GetPermissionFullName(LpAuthPermissionType type, string name) =>
            AuthorizationHelper.GetPermissionFullName(type, name);
        public string GetPermissionName(string fullName) =>
            AuthorizationHelper.GetPermissionName(fullName);
        public LpAuthPermissionType GetPermissionType(string fullName) =>
            AuthorizationHelper.GetPermissionType(fullName);
        #endregion

        #region HasAccess
        public bool HasAccess(LpAuthRequirement requirement, List<LpAuthPermission> permissions)
        {
            if (permissions.IsNullOrEmpty())
            {
                return false;
            }

            if (string.IsNullOrEmpty(requirement?.PermissionFullName))
            {
                return false;
            }

            if (permissions.Any(p => GetPermissionType(p.FullName) == LpAuthPermissionType.Root))
            {
                return true;
            }

            var type = GetPermissionType(requirement.PermissionFullName);
            var actuals = permissions
                .Where(p => requirement.PermissionFullName.Equals(
                    p.FullName,
                    StringComparison.Ordinal))
                .Select(p => p.ActualValue)
                .ToList();
            if (actuals.Count == 0)
            {
                return false;
            }

            return HasAccess(type, actuals, requirement.ExpectedValue);
        }

        public bool HasAccess(
            List<LpAuthRequirement> requirements,
            List<LpAuthPermission> permissions)
        {
            if (requirements.IsNullOrEmpty() || permissions.IsNullOrEmpty())
            {
                return false;
            }

            if (permissions.Any(p => GetPermissionType(p.FullName) == LpAuthPermissionType.Root))
            {
                return true;
            }

            var actualData = permissions
                .GroupBy(p => p.FullName)
                .ToDictionary(g => g.Key, g => g.Select(p => p.ActualValue).ToList());
            var data = requirements
                .Where(r => actualData.ContainsKey(r.PermissionFullName))
                .Select(r => new
                {
                    Type = GetPermissionType(r.PermissionFullName),
                    Expected = r.ExpectedValue,
                    Actuals = actualData[r.PermissionFullName]
                })
                .ToList();
            if (data.Count == 0)
            {
                return false;
            }

            return data.All(d => HasAccess(d.Type, d.Actuals, d.Expected));
        }
        #endregion

        #region Internals
        private bool HasAccess(LpAuthPermissionType type, List<object> actuals, object expected)
        {
            if (type == LpAuthPermissionType.Binary)
            {
                return actuals.Any(a => a is bool allow && allow);
            }

            if (type == LpAuthPermissionType.Level)
            {
                if (expected is not int expectedLevel || expectedLevel < 2)
                {
                    return false;
                }

                return actuals.Any(a => a is int actualLevel && actualLevel >= expectedLevel);
            }

            // NOTE: Passkey type
            if (expected is not HashSet<string> expectedPasskeys)
            {
                return false;
            }

            return actuals.Any(a => a is HashSet<string> actualPasskeys &&
                expectedPasskeys.Intersect(actualPasskeys, StringComparer.Ordinal).Any());
        }
        #endregion
    }
}
