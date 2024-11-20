using System;

namespace LuckyProject.AuthServer.Services.LpApi.Responses
{
    public class ApiResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Endpoint { get; init; }
        public string CallbackUrl { get; init; }
        public string MachineClientId { get; init; }
        public Guid MachineUserId { get; init; }
    }
}
