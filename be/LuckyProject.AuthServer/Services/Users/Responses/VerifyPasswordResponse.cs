using System;

namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class VerifyPasswordResponse
    {
        public UserResponse User { get; init; }
        public bool IsLockedOut { get; init; }
        public bool CredsValid { get; init; }
        public Guid? TwoFactorRequestId { get; init; }
    }
}
