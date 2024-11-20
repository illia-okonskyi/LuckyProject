using LuckyProject.Lib.Basics.LiveObjects.Sessions;
using System;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpSessionManagerFactory : ILpSessionManagerFactory
    {
        #region Internals & ctor
        private readonly ILpInMemoryCacheFactory cacheFactory;

        public LpSessionManagerFactory(ILpInMemoryCacheFactory cacheFactory)
        {
            this.cacheFactory = cacheFactory;
        }
        #endregion

        #region Public interface
        public ILpSessionManager<TService, TStorage, TSession, TContext> CreateSessionManager<
            TService,
            TStorage,
            TSession,
            TContext>(
                TService service,
                TStorage storage)
        where TService : ILpSessionService<TSession, TContext>
        where TStorage : ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
        {
            return new LpSessionManager<TService, TStorage, TSession, TContext>(service, storage);
        }

        public ILpSessionManager<
            TService,
            LpInMemoryCacheSessionStorage<TSession, TContext>,
            TSession,
            TContext> CreateInMemorySessionManager<TService, TSession, TContext>(
                TService service,
                TimeSpan expireInterval,
                TimeSpan checkInterval)
        where TService : ILpSessionService<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new()
        {
            return new LpSessionManager<
                TService,
                LpInMemoryCacheSessionStorage<TSession, TContext>,
                TSession,
                TContext>(
                    service,
                    new LpInMemoryCacheSessionStorage<TSession, TContext>(
                        cacheFactory,
                        expireInterval,
                        checkInterval));
        }
        #endregion
    }
}
