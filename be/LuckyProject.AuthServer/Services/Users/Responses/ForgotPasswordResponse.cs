using System;

namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class ForgotPasswordResponse
    {
        public UserResponse User { get; init; }
        public bool IsLockedOut { get; init; }
        public bool UserFound { get; init; }
        public Guid? RequestId { get; init; }
    }
}
