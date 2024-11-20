using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public interface ILpSessionService<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync();
        Task<TSession> CreateSessionAsync(
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default);
        Task PingSessionAsync(
            TSession session,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default);
        Task UpdateSessionAsync(
            TSession session,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default);
        Task DeleteSessionAsync(
            TSession session,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// <paramref name="context"/> is null when deleting expired sessions
        /// </summary>
        Task DeleteSessionsAsync(
            List<TSession> sessions,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default);
    }
}
