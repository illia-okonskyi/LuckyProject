using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public interface ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
    {
        Task StartAsync(
            Func<List<TSession>, CancellationToken, Task> expiredSessionsCallbackAsync,
            CancellationToken cancellationToken = default);
        Task StopAsync();
        Task WriteSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// Must return null if not found
        /// </summary>
        Task<TSession> ReadSessionAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Must return at least empty list
        /// </summary>
        Task<List<TSession>> ReadSessionsAsync(
            object context,
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
            object context = default,
            CancellationToken cancellationToken = default);
        Task DeleteSessionsAsync(
            List<TSession> sessions,
            object context = default,
            CancellationToken cancellationToken = default);
    }
}
