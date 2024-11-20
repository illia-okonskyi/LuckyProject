using System;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class CheckPasswordRequest
    {
        public Guid Id { get; init; }
        public string Password { get; init; }
    }
}
