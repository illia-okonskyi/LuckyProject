using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public class LpSessionManager<TService, TStorage, TSession, TContext>
        : ILpSessionManager<TService, TStorage, TSession, TContext>
        where TService : ILpSessionService<TSession, TContext>
        where TStorage : ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
    {
        #region Internals & ctor
        private readonly TService service;
        private readonly TStorage storage;

        public LpSessionManager(TService service, TStorage storage)
        {
            this.service = service;
            this.storage = storage;
        }
        #endregion

        #region Public interface
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await storage.StartAsync(OnSessionsExpiredAsync, cancellationToken);
            await service.StartAsync(cancellationToken);
        }

        public async Task StopAsync()
        {
            await service.StopAsync();
            await storage.StopAsync();
        }

        public async Task<TSession> CreateSessionAsync(
            object context = default,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            var session = await service.CreateSessionAsync(utcNow, context, cancellationToken);
            await storage.WriteSessionAsync(session, context, cancellationToken);
            return session;
        }

        public async Task<TSession> GetSessionAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await storage.ReadSessionAsync(id, cancellationToken);
        }

        public async Task<List<TSession>> GetSessionsAsync(
            object context,
            CancellationToken cancellationToken = default)
        {
            return await storage.ReadSessionsAsync(context, cancellationToken);
        }

        public async Task PingSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            await storage.PingSessionAsync(session, utcNow, context, cancellationToken);
            await service.PingSessionAsync(session, utcNow, context, cancellationToken);
        }

        public async Task UpdateSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            await storage.PingSessionAsync(session, utcNow, context, cancellationToken);
            await service.UpdateSessionAsync(session, utcNow, context, cancellationToken);
            await storage.UpdateSessionAsync(session, utcNow, context, cancellationToken);
        }

        public async Task DeleteSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            await storage.DeleteSessionAsync(session, context, cancellationToken);
            await service.DeleteSessionAsync(session, utcNow, context, cancellationToken);
        }

        public async Task DeleteSessionsAsync(
            List<TSession> sessions,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            var utcNow = DateTime.UtcNow;
            await storage.DeleteSessionsAsync(sessions, context, cancellationToken);
            await service.DeleteSessionsAsync(sessions, utcNow, context, cancellationToken);
        }
        #endregion

        #region Internals
        private async Task OnSessionsExpiredAsync(
            List<TSession> sessions,
            CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;
            await service.DeleteSessionsAsync(sessions, utcNow, null, cancellationToken);
        }
        #endregion
    }
}
