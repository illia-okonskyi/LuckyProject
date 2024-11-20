using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public interface ILpSessionManager<TService, TStorage, TSession, TContext>
        where TService : ILpSessionService<TSession, TContext>
        where TStorage : ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
        Task<TSession> CreateSessionAsync(
            object context = default,
            CancellationToken cancellationToken = default);
        Task<TSession> GetSessionAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<TSession>> GetSessionsAsync(
            object context,
            CancellationToken cancellationToken = default);
        Task PingSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default);
        Task UpdateSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default);
        Task DeleteSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default);
        Task DeleteSessionsAsync(
            List<TSession> sessions,
            object context = default,
            CancellationToken cancellationToken = default);
    }
}
