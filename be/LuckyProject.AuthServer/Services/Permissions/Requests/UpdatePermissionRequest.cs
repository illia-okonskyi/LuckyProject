using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Permissions.Requests
{
    public class UpdatePermissionRequest
    {
        public Guid Id { get; set; }
        public string Description { get; init; }
        public bool IgnoreSealed { get; set; }
        public bool? Allow { get; init; }
        public int? Level { get; init; }
        public HashSet<string> Passkeys { get; init; }

        #region Internal Service usage
        public LpAuthPermissionType? Type { get; private set; }

        public void PrepareRequest(ILpAuthorizationService authService, string fullName)
        {
            Type = authService.GetPermissionType(fullName);
        }
        #endregion

    }
}
