using LuckyProject.AuthServer.Services.OpenId.Models;
using LuckyProject.Lib.Web.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.OpenId.Responses
{
    public class ClientResponse
    {
        public class WebDetails
        {
            public HashSet<WebClientOrigin> Origins { get; init; }
        }

        public class MachineDetails
        {
            public Guid UserId { get; init; }
            public HashSet<Uri> Origins { get; init; }
        }

        public Guid Id { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LpApiClientType Type { get; init; }
        public string Name { get; init; }
        public string ClientId { get; init; }
        public string DisplayName { get; init; }
        public bool IsSealed { get; init; }
        public WebDetails Web { get; init; }
        public MachineDetails Machine { get; init; }
    }
}
