using LuckyProject.Lib.Basics.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Sessions
{
    public interface ILpInMemoryCacheSessionStorage<TSession, TContext>
        : ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new ()
    {
    }

    public class LpInMemoryCacheSessionStorage<TSession, TContext>
        : ILpInMemoryCacheSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
    {
        #region Internals & ctor
        private readonly ILpInMemoryCache<Guid, TSession> cache;
        private Func<List<TSession>, CancellationToken, Task> expiredSessionsCallbackAsync;

        public LpInMemoryCacheSessionStorage(
            ILpInMemoryCacheFactory cacheFactory,
            TimeSpan expireInterval,
            TimeSpan checkInterval)
        {
            cache = cacheFactory.CreateCache<Guid, TSession>(
                expireInterval,
                OnCacheEntriesExpired,
                checkInterval);
        }
        #endregion

        #region Public interface
        public Task StartAsync(
            Func<List<TSession>, CancellationToken, Task> expiredSessionsCallbackAsync,
            CancellationToken cancellationToken = default)
        {
            this.expiredSessionsCallbackAsync = expiredSessionsCallbackAsync;
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            cache.Clear();
            return Task.CompletedTask;
        }

        public Task WriteSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            cache.Add(session.Id, session);
            return Task.CompletedTask;
        }

        public Task<TSession> ReadSessionAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (!cache.Contains(id))
            {
                return Task.FromResult(default(TSession));
            }

            return Task.FromResult(cache.Get(id));
        }

        /// <summary>
        /// Accepts <![CDATA[Func<Guid, TSession, bool>]]> (predicate) as <paramref name="context"/>
        /// </summary>
        public Task<List<TSession>> ReadSessionsAsync(
            object context,
            CancellationToken cancellationToken = default)
        {
            if (context is not Func<Guid, TSession, bool> pred)
            {
                return Task.FromResult(new List<TSession>());
            }
            return Task.FromResult(cache.GetRange(pred));
        }

        public Task PingSessionAsync(
            TSession session,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            cache.GetAndUpdateExpiration(session.Id);
            session.ExpiresAtUtc = utcNow + cache.DefaultExpireInterval;
            return Task.CompletedTask;
        }

        public Task UpdateSessionAsync(
            TSession session,
            DateTime utcNow,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            cache.Update(session.Id, session);
            return Task.CompletedTask;
        }

        public Task DeleteSessionAsync(
            TSession session,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            cache.Delete(session.Id);
            return Task.CompletedTask;
        }

        public Task DeleteSessionsAsync(
            List<TSession> sessions,
            object context = default,
            CancellationToken cancellationToken = default)
        {
            cache.DeleteRange(sessions.Select(s => s.Id));
            return Task.CompletedTask;
        }
        #endregion

        #region Internals
        private async Task OnCacheEntriesExpired(List<TSession> sessions, CancellationToken ct)
        {
            await expiredSessionsCallbackAsync(sessions, ct);
        }
        #endregion
    }
}
