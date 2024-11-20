using System;

namespace LuckyProject.AuthServer.Services.Init
{
    public class InitialSeedOptions
    {
        public class SaSeed
        {
            public string PermissionName { get; set; }
            public string RoleName { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string FullName { get; set; }
            public string Password { get; set; }
            public string TelegramUserName { get; set; }
            public string PreferredLocale { get; set; }

        }

        public class AuthServerApiSeed
        {
            public string PermissionName { get; set; }
            public string RoleName { get; set; }
        }

        public class DefaultWebClientSeed
        {
            public Uri BaseUrl { get; set; }
            public string RedirectPath { get; set; }
            public string PostLogoutRedirectPath { get; set; }
        }

        public SaSeed Sa { get; set; }
        public AuthServerApiSeed AuthServerApi { get; set; }
        public DefaultWebClientSeed DefaultWebClient { get; set; }
    }
}
