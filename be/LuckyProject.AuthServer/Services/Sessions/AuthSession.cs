using LuckyProject.Lib.Basics.LiveObjects.Sessions;
using System;

namespace LuckyProject.AuthServer.Services.Sessions
{
    public class AuthSessionContext
    {
        public string ClientId { get; init; }
        public Guid UserId { get; init; }
        public string NotificationsHubConnectionId { get; set; }
    }

    public class AuthSession : LpSession<AuthSessionContext>
    {
        public AuthSession(
            DateTime createdAtUtc,
            AuthSessionContext context,
            TimeSpan expiration)
            : base(createdAtUtc, context, expiration)
        { }
    }
}
