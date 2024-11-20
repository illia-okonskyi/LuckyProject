using System;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class ResetPasswordRequest
    {
        public Guid Id { get; init; }
        public string NewPassword { get; init; }
        public string NewPasswordRepeat { get; init; }
    }
}
