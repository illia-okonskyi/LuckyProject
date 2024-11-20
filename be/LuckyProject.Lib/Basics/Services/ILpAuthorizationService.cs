using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpAuthorizationService
    {
        #region Name/type operations
        string GetPermissionTypePrefix(LpAuthPermissionType type);
        string GetPermissionFullName(LpAuthPermissionType type, string name);
        string GetPermissionName(string fullName);
        LpAuthPermissionType GetPermissionType(string fullName);
        #endregion

        #region HasAccess
        bool HasAccess(LpAuthRequirement requirement, List<LpAuthPermission> permissions);
        bool HasAccess(
            List<LpAuthRequirement> requirements,
            List<LpAuthPermission> permissions);
        #endregion
    }
}
