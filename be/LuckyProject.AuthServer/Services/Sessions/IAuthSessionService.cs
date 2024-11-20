using LuckyProject.Lib.Basics.LiveObjects.Sessions;
using System;

namespace LuckyProject.AuthServer.Services.Sessions
{
    public interface IAuthSessionService : ILpSessionService<AuthSession, AuthSessionContext>
    {
        public class CreateContext
        {
            public string ClientId { get; init; }
            public Guid UserId { get; init; }
        }
    }
}
