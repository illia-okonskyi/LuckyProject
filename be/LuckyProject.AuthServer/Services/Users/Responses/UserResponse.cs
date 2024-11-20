using System;

namespace LuckyProject.AuthServer.Services.Users.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; init; }
        public string NormalizedUserName { get; init; }
        public string Email { get; init; }
        public string NormalizedEmail { get; init; }
        public string PhoneNumber { get; init; }
        public string FullName { get; init; }
        public string TelegramUserName { get; init; }
        public string PreferredLocale { get; init; }
        public string PreferredLocaleDisplayName { get; set; }
        public string MachineClientId { get; init; }
        public bool IsSealed { get; init; }
        public bool IsMachineUser => !string.IsNullOrEmpty(MachineClientId);
        public DateTimeOffset? LockoutEndUtc { get; set; }
        public bool IsLockedOut =>
            LockoutEndUtc.HasValue && LockoutEndUtc.Value >= DateTimeOffset.UtcNow;
    }
}
