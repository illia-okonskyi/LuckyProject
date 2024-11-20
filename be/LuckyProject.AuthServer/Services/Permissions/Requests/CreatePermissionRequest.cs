using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Permissions.Requests
{
    public class CreatePermissionRequest
    {
        public LpAuthPermissionType Type { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public bool IsSealed { get; init; }
        public bool? Allow { get; init; }
        public int? Level { get; init; }
        public HashSet<string> Passkeys { get; init; }
        public Guid? ApiId { get; init; }

        #region Internal Service usage
        public string FullName { get; private set; }

        public void PrepareRequest(ILpAuthorizationService authService)
        {
            FullName = authService.GetPermissionFullName(Type, Name);
        }
        #endregion
    }
}
