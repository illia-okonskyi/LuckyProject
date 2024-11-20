using System.Collections.Generic;
using LuckyProject.AuthServer.Services.OpenId.Models;

namespace LuckyProject.AuthServer.Services.OpenId.Requests
{
    public class CreateWebClientRequest
    {
        public string Name { get; init; }
        public string DisplayName { get; init; }
        public HashSet<WebClientOrigin> Origins { get; init; }
        public bool IsSealed { get; init; }

        #region Internal Service usage
        public string ClientId { get; set; }
        #endregion
    }
}
