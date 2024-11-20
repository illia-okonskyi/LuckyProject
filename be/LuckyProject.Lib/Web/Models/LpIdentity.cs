using LuckyProject.Lib.Web.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace LuckyProject.Lib.Web.Models
{
    public class LpIdentity
    {
        public string ClientId { get; set; }
        public LpApiClientType ClientType { get; set; }
        public string ClientName { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public Guid SessionId { get; set;} = Guid.Empty;
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserFullName { get; set; }
        public string UserTelegramUserName { get; set; }
        public string UserPreferredLocale { get; set; }
        public List<Guid> UserRoleIds { get; set; } = new();

        public ClaimsIdentity ToClaimsIdentity()
        {
            var claims = new List<Claim>
            {
                new Claim(LpWebConstants.Auth.Jwt.Claims.Subject, UserId.ToString()),
                new Claim(LpWebConstants.Auth.Jwt.Claims.ClientId, ClientId),
                new Claim(LpWebConstants.Auth.Jwt.Claims.SessionId, SessionId.ToString()),
            };

            if (!string.IsNullOrEmpty(UserName))
            {
                claims.Add(new Claim(LpWebConstants.Auth.Jwt.Claims.UserName, UserName));
            }

            if (!string.IsNullOrEmpty(UserEmail))
            {
                claims.Add(new Claim(LpWebConstants.Auth.Jwt.Claims.Email, UserEmail));
            }

            if (!string.IsNullOrEmpty(UserPhoneNumber))
            {
                claims.Add(new Claim(LpWebConstants.Auth.Jwt.Claims.PhoneNumber, UserPhoneNumber));
            }

            if (!string.IsNullOrEmpty(UserFullName))
            {
                claims.Add(new Claim(LpWebConstants.Auth.Jwt.Claims.FullName, UserFullName));
            }

            if (!string.IsNullOrEmpty(UserTelegramUserName))
            {
                claims.Add(new Claim(
                    LpWebConstants.Auth.Jwt.Claims.TelegramUserName,
                    UserTelegramUserName));
            }

            if (!string.IsNullOrEmpty(UserPreferredLocale))
            {
                claims.Add(new Claim(
                    LpWebConstants.Auth.Jwt.Claims.PreferredLocale,
                    UserPreferredLocale));
            }

            if (UserRoleIds.Count > 0)
            {
                claims.AddRange(UserRoleIds
                    .Select(id => new Claim(LpWebConstants.Auth.Jwt.Claims.Role, id.ToString())));
            }

            return new ClaimsIdentity(claims, LpWebConstants.Auth.Jwt.AuthenticationScheme);
        }
    }
}
