using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Sessions
{
    public class AuthSessionService : IAuthSessionService
    {
        #region Internals & ctor
        private readonly AuthSessionOptions options;

        public AuthSessionService(IOptions<AuthSessionOptions> options)
        {
            this.options = options.Value;
        }
        #endregion

        #region Public interface
        public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task StopAsync() => Task.CompletedTask;

        public Task<AuthSession> CreateSessionAsync(
            DateTime utcNow,
            object context = null,
            CancellationToken cancellationToken = default)
        {
            if (context is not IAuthSessionService.CreateContext cc)
            {
                throw new ArgumentException("Invalid context");
            }

            return Task.FromResult(new AuthSession(
                utcNow,
                new() { ClientId = cc.ClientId, UserId = cc.UserId },
                options.ExpireInterval));
        }

        public Task PingSessionAsync(
            AuthSession session,
            DateTime utcNow,
            object context = null,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateSessionAsync(
            AuthSession session,
            DateTime utcNow,
            object context = null,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteSessionAsync(
            AuthSession session,
            DateTime utcNow,
            object context = null,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task DeleteSessionsAsync(
            List<AuthSession> sessions,
            DateTime utcNow,
            object context = null,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
        #endregion
    }
}
