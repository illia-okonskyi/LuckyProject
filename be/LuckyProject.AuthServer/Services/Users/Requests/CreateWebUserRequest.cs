using LuckyProject.AuthServer.DbLayer;
using Microsoft.AspNetCore.Identity;

namespace LuckyProject.AuthServer.Services.Users.Requests
{
    public class CreateWebUserRequest
    {
        public string UserName { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public string FullName { get; init; }
        public string Password { get; init; }
        public string TelegramUserName { get; init; }
        public string PreferredLocale { get; init; }
        public bool IsSealed { get; init; }

        #region Internal Service usage
        public string NormalizedUserName { get; private set; }
        public string NormalizedEmail { get; private set; }
        public AuthServerUser User { get; private set; }


        public void PrepareRequest(ILookupNormalizer normalizer)
        {
            NormalizedUserName = normalizer.NormalizeName(UserName);
            NormalizedEmail = normalizer.NormalizeEmail(Email);
            User = new()
            {
                UserName = UserName,
                Email = Email
            };
        }
        #endregion
    }
}
