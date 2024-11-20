using LuckyProject.Lib.Basics.LiveObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpInMemoryCacheFactory
    {
        ILpInMemoryCache<TValue> CreateCache<TValue>(
            TimeSpan defaultExpireInterval,
            IEqualityComparer<TValue> comparer = null,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null);
        ILpInMemoryCache<TKey, TValue> CreateCache<TKey, TValue>(
            TimeSpan defaultExpireInterval,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null)
            where TKey : notnull;
    }
}
