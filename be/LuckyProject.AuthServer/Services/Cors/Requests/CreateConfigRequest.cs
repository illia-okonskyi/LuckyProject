using System;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Cors.Requests
{
    public class CreateConfigRequest
    {
        public string ClientId { get; init; }
        public HashSet<Uri> Origins { get; init; }
    }
}
