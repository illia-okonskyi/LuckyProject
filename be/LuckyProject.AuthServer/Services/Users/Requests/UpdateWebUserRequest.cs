using Microsoft.AspNetCore.Identity;
using System;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class UpdateWebUserRequest
    {
        public Guid Id { get; set; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FullName { get; init; }
        public string TelegramUserName { get; init; }
        public string PreferredLocale { get; init; }

        #region Internal Service usage
        public string NormalizedEmail { get; private set; }

        public void PrepareRequest(ILookupNormalizer normalizer)
        {
            NormalizedEmail = normalizer.NormalizeEmail(Email);
        }
        #endregion
    }
}
