using LuckyProject.Lib.Basics.LiveObjects.Sessions;
using System;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpSessionManagerFactory
    {
        ILpSessionManager<TService, TStorage, TSession, TContext> CreateSessionManager<
            TService,
            TStorage,
            TSession,
            TContext>(
                TService service,
                TStorage storage)
        where TService : ILpSessionService<TSession, TContext>
        where TStorage : ILpSessionStorage<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new();

        ILpSessionManager<
            TService,
            LpInMemoryCacheSessionStorage<TSession, TContext>,
            TSession,
            TContext> CreateInMemorySessionManager<TService, TSession, TContext>(
                TService service,
                TimeSpan expireInterval,
                TimeSpan checkInterval)
        where TService : ILpSessionService<TSession, TContext>
        where TSession : ILpSession<TContext>
        where TContext : class, new();
    }
}
