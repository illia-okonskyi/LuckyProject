using Microsoft.AspNetCore.Identity;
using System;

namespace LuckyProject.AuthServer.Services.Roles.Requests
{
    public class CreateUpdateRoleRequest
    {
        /// <summary>
        /// Is appliable only for update
        /// </summary>
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        /// <summary>
        /// Is appliable only for create
        /// </summary>
        public bool IsSealed { get; init; }
        /// <summary>
        /// Is appliable only for update
        /// </summary>
        public bool IgnoreSealed { get; init; }

        #region Internal Service usage
        public string NormalizedName { get; private set; }

        public void PrepareRequest(ILookupNormalizer normalizer)
        {
            NormalizedName = normalizer.NormalizeName(Name);
        }
        #endregion
    }
}
