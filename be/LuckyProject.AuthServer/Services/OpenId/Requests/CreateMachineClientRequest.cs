using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.OpenId.Requests
{
    public class CreateMachineClientRequest
    {
        public string Name { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string PreferredLocale { get; init; }
        public HashSet<Uri> Origins { get; init; }
        public string Secret { get; init; }
        public bool IsSealed { get; init; }

        #region Internal Service usage
        public string ClientId { get; set; }
        #endregion
    }
}
