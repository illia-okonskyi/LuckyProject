using Microsoft.AspNetCore.Identity;
using System;

namespace LuckyProject.AuthServer.DbLayer
{
    public class AuthServerUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string MachineClientId { get; set; }
        public string TelegramUserName { get; set; }
        public string PreferredLocale { get; set; }
        public bool IsSealed { get; set; }

        public LpApi Api { get; set; }
    }
}
