using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.LpApi.Responses
{
    public class UserFeaturesResponse
    {
        public class ApiResponse()
        {
            public string Name { get; init; }
            public string Endpoint { get; init; }
            public List<string> Features { get; init; } = new();
        }

        public List<ApiResponse> Apis { get; init; } = new();
    }
}
