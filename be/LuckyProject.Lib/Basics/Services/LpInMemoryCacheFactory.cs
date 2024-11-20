using LuckyProject.Lib.Basics.LiveObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpInMemoryCacheFactory : ILpInMemoryCacheFactory
    {
        private readonly IThreadSyncService tsService;
        private readonly ILpTimerFactory timerFactory;

        public LpInMemoryCacheFactory(IThreadSyncService tsService, ILpTimerFactory timerFactory)
        {
            this.tsService = tsService;
            this.timerFactory = timerFactory;
        }

        public ILpInMemoryCache<TValue> CreateCache<TValue>(
            TimeSpan defaultExpireInterval,
            IEqualityComparer<TValue> comparer = null,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null)
        {
            return new LpInMemoryCache<TValue>(
                tsService,
                timerFactory,
                defaultExpireInterval,
                comparer,
                expiredCallbackAsync,
                checkInterval);
        }

        public ILpInMemoryCache<TKey, TValue> CreateCache<TKey, TValue>(
            TimeSpan defaultExpireInterval,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null)
            where TKey : notnull
        {
            return new LpInMemoryCache<TKey, TValue>(
                tsService,
                timerFactory,
                defaultExpireInterval,
                expiredCallbackAsync,
                checkInterval);
        }
    }
}
