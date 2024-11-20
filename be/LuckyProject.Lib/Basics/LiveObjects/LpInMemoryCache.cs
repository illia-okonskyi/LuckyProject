using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    #region CacheEntry
    internal class CacheEntry<TValue>
    {
        public TValue Value { get; set; }
        public DateTime ExpiresAt { get; set; }
        public IEqualityComparer<TValue> Comparer { get; init; }

        public override bool Equals(object obj)
        {
            var comparer = Comparer ?? EqualityComparer<TValue>.Default;
            return obj is CacheEntry<TValue> entry && comparer.Equals(Value, entry.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
    #endregion

    #region LpInMemoryCache<TValue>
    public class LpInMemoryCache<TValue> : ILpInMemoryCache<TValue>
    {
        #region Internals & ctor & Dispose
        private readonly IThreadSyncService tsService;

        private readonly HashSet<CacheEntry<TValue>> cache = new();
        private readonly ReaderWriterLockSlim rwlsCache = new();
        private readonly Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync;
        private readonly ILpTimer timer;

        public LpInMemoryCache(
            IThreadSyncService tsService,
            ILpTimerFactory timerFactory,
            TimeSpan defaultExpireInterval,
            IEqualityComparer<TValue> comparer = null,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null)
        {
            this.tsService = tsService;
            DefaultExpireInterval = defaultExpireInterval;
            Comparer = comparer;
            this.expiredCallbackAsync = expiredCallbackAsync;
            checkInterval ??= TimeoutDefaults.Medium;
            timer = timerFactory.CreateTimer(checkInterval.Value, false, OnTimerAsync);
        }

        public void Dispose()
        {
            timer.Dispose();
        }
        #endregion

        #region Public interface
        public TimeSpan DefaultExpireInterval { get; }
        public IEqualityComparer<TValue> Comparer { get; }

        public void Add(TValue value, TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache,
                () => cache.Add(new CacheEntry<TValue>
                {
                    Value = value,
                    ExpiresAt = GetExpiresAt(expireInterval),
                    Comparer = Comparer
                }));
        }

        public void AddRange(IEnumerable<TValue> values, TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var expiresAt = GetExpiresAt(expireInterval);
                foreach (var v in values)
                {
                    cache.Add(new CacheEntry<TValue>
                    {
                        Value = v,
                        ExpiresAt = expiresAt,
                        Comparer = Comparer
                    });
                }
            });
        }

        public bool Contains(TValue value)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache.Contains(new CacheEntry<TValue> { Value = value}));
            return result;
        }

        public bool ContainsAny(IEnumerable<TValue> values)
        {
            var entries = values.Select(v => new CacheEntry<TValue> { Value = v }).ToList();
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => entries.Any(e => cache.Contains(e)));
            return result;
        }

        public bool ContainsAll(IEnumerable<TValue> values)
        {
            var entries = values.Select(v => new CacheEntry<TValue> { Value = v }).ToList();
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => entries.All(e => cache.Contains(e)));
            return result;
        }

        public TValue GetOrDefault(Func<TValue, bool> pred)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(rwlsCache, () =>
            {
                var entry = cache.FirstOrDefault(e => pred(e.Value));
                return entry != null ? entry.Value : default;
            });
            return result;
        }

        public List<TValue> GetRange(Func<TValue, bool> pred)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache.Where(e => pred(e.Value)).Select(e => e.Value).ToList());
            return result;
        }

        public TValue GetAndUpdateExpirationOrDefault(
            Func<TValue, bool> pred,
            TimeSpan? expireInterval = null)
        {
            var (_, result) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entry = cache.FirstOrDefault(e => pred(e.Value));
                if (entry == null)
                {
                    return default;
                }
                
                entry.ExpiresAt = GetExpiresAt(expireInterval);
                return entry.Value;
            });
            return result;
        }

        public List<TValue> GetRangeAndUpdateExpiration(
            Func<TValue, bool> pred,
            TimeSpan? expireInterval = null)
        {
            var expiresAt = GetExpiresAt(expireInterval);
            var (_, result) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var r = new List<TValue>();
                foreach (var e in cache)
                {
                    if (!pred(e.Value))
                    {
                        continue;
                    }

                    e.ExpiresAt = expiresAt;
                    r.Add(e.Value);
                }
                return r;
            });
            return result;
        }

        public void Update(
            Func<TValue, bool> pred,
            TValue value,
            TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entry = cache.FirstOrDefault(e => pred(e.Value));
                if (entry == null)
                {
                    return;
                }

                entry.ExpiresAt = GetExpiresAt(expireInterval);
                entry.Value = value;
            });
        }

        public void UpdateRange(
            Func<TValue, (bool, TValue)> pred,
            TimeSpan? expireInterval = null)
        {
            var expiresAt = GetExpiresAt(expireInterval);
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                foreach (var e in cache)
                {
                    var (u, v) = pred(e.Value);
                    if (!u)
                    {
                        continue;
                    }

                    e.ExpiresAt = expiresAt;
                    e.Value = v;
                }
            });
        }

        public void Delete(TValue value)
        {
            tsService.ReaderWriterLockSlimWriteGuard(
                rwlsCache,
                () => cache.Remove(new CacheEntry<TValue> { Value = value }));
        }

        public void DeleteRange(Func<TValue, bool> pred)
        {
            tsService.ReaderWriterLockSlimWriteGuard(
                rwlsCache,
                () => cache.Where(e => pred(e.Value)).ToList().ForEach(e => cache.Remove(e)));
        }
        public void Clear()
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, cache.Clear);
        }
        #endregion

        #region Internals
        private DateTime GetExpiresAt(TimeSpan? invalidateInterval)
        {
            invalidateInterval ??= DefaultExpireInterval;
            return DateTime.Now + invalidateInterval.Value;
        }

        private async Task OnTimerAsync(DateTime dt, CancellationToken ct)
        {
            var now = DateTime.Now;
            var (_, expiredEntries) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entries = cache.Where(e => e.ExpiresAt >= now).ToList();
                entries.ForEach(e => cache.Remove(e));
                return entries;
            });

            await expiredCallbackAsync(expiredEntries.Select(e => e.Value).ToList(), ct);
        }
        #endregion
    }
    #endregion

    #region LpInMemoryCache<TKey, TValue>
    public class LpInMemoryCache<TKey, TValue> : ILpInMemoryCache<TKey, TValue>
        where TKey : notnull
    {
        #region Internals & ctor & Dispose
        private readonly IThreadSyncService tsService;

        private readonly Dictionary<TKey, CacheEntry<TValue>> cache = new();
        private readonly ReaderWriterLockSlim rwlsCache = new();
        private readonly Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync;
        private readonly ILpTimer timer;

        public LpInMemoryCache(
            IThreadSyncService tsService,
            ILpTimerFactory timerFactory,
            TimeSpan defaultInvalidateInterval,
            Func<List<TValue>, CancellationToken, Task> expiredCallbackAsync = null,
            TimeSpan? checkInterval = null)
        {
            this.tsService = tsService;
            DefaultExpireInterval = defaultInvalidateInterval;
            this.expiredCallbackAsync = expiredCallbackAsync;
            checkInterval ??= TimeoutDefaults.Medium;
            timer = timerFactory.CreateTimer(checkInterval.Value, false, OnTimerAsync);
        }

        public void Dispose()
        {
            timer.Dispose();
        }
        #endregion

        #region Public interface
        public TimeSpan DefaultExpireInterval { get; }
        public void Add(TKey key, TValue value, TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache,
                () => cache.Add(
                    key,
                    new CacheEntry<TValue>
                    {
                        Value = value,
                        ExpiresAt = GetExpiresAt(expireInterval)
                    }));
        }

        public void AddRange(
            IEnumerable<KeyValuePair<TKey, TValue>> values,
            TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var expiresAt = GetExpiresAt(expireInterval);
                foreach (var kvp in values)
                {
                    cache.Add(
                        kvp.Key,
                        new CacheEntry<TValue>
                        {
                            Value = kvp.Value,
                            ExpiresAt = expiresAt
                        });
                }
            });
        }

        public bool Contains(TKey key)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache.ContainsKey(key));
            return result;
        }

        public bool ContainsAny(IEnumerable<TKey> keys)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => keys.Any(cache.ContainsKey));
            return result;
        }

        public bool ContainsAll(IEnumerable<TKey> keys)
        {
            var (_, result) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => keys.All(cache.ContainsKey));
            return result;
        }

        public TValue Get(TKey key)
        {
            var (_, value) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache[key].Value);
            return value;
        }

        public TValue GetOrDefault(Func<TKey, TValue, bool> pred)
        {
            var (_, value) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache.FirstOrDefault(kvp => pred(kvp.Key, kvp.Value.Value)));
            return value.Value.Value;
        }

        public List<TValue> GetRange(IEnumerable<TKey> keys)
        {
            var (_, values) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => keys.Select(k => cache[k].Value).ToList());
            return values;
        }

        public List<TValue> GetRange(Func<TKey, TValue, bool> pred)
        {
            var (_, values) = tsService.ReaderWriterLockSlimReadGuard(
                rwlsCache,
                () => cache.Where(kvp => pred(kvp.Key, kvp.Value.Value)).ToList());
            return values.Select(kvp => kvp.Value.Value).ToList();
        }

        public TValue GetAndUpdateExpiration(TKey key, TimeSpan? expireInterval = null)
        {
            var (_, value) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entry = cache[key];
                entry.ExpiresAt = GetExpiresAt(expireInterval);
                return entry.Value;
            });
            return value;
        }

        public List<TValue> GetRangeAndUpdateExpiration(
            IEnumerable<TKey> keys,
            TimeSpan? expireInterval = null)
        {
            var (_, values) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var expiersAt = GetExpiresAt(expireInterval);
                var result = new List<TValue>();
                foreach (var k in keys)
                {
                    var entry = cache[k];
                    entry.ExpiresAt = expiersAt;
                    result.Add(entry.Value);
                }
                return result;
            });
            return values;
        }

        public void Update(TKey key, TValue value, TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entry = cache[key];
                entry.Value = value;
                entry.ExpiresAt = GetExpiresAt(expireInterval);
            });
        }

        public void UpdateRange(
            IEnumerable<KeyValuePair<TKey, TValue>> values,
            TimeSpan? expireInterval = null)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var expiersAt = GetExpiresAt(expireInterval);
                foreach (var kvp in values)
                {
                    var entry = cache[kvp.Key];
                    entry.Value = kvp.Value;
                    entry.ExpiresAt = expiersAt;
                }
            });
        }

        public void Delete(TKey key)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () => cache.Remove(key));
        }

        public void DeleteRange(IEnumerable<TKey> keys)
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                foreach (var k in keys)
                {
                    cache.Remove(k);
                }
            });
        }

        public void Clear()
        {
            tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, cache.Clear);
        }
        #endregion

        #region Internals
        private DateTime GetExpiresAt(TimeSpan? invalidateInterval)
        {
            invalidateInterval ??= DefaultExpireInterval;
            return DateTime.Now + invalidateInterval.Value;
        }

        private async Task OnTimerAsync(DateTime dt, CancellationToken ct)
        {
            var now = DateTime.Now;
            var (_, expiredEntries) = tsService.ReaderWriterLockSlimWriteGuard(rwlsCache, () =>
            {
                var entries = cache.Where(kvp => kvp.Value.ExpiresAt >= now).ToList();
                entries.ForEach(e => cache.Remove(e.Key));
                return entries;
            });

            await expiredCallbackAsync(expiredEntries.Select(kvp => kvp.Value.Value).ToList(), ct);
        }
        #endregion
    }
    #endregion
}
