using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    #region ILpInMemoryCache
    public interface ILpInMemoryCache: IDisposable
    {
        TimeSpan DefaultExpireInterval { get; }
    }
    #endregion

    #region ILpInMemoryCache<TValue>
    public interface ILpInMemoryCache<TValue> : ILpInMemoryCache
    {
        public IEqualityComparer<TValue> Comparer { get; }

        void Add(TValue value, TimeSpan? expireInterval = null);
        void AddRange(IEnumerable<TValue> values, TimeSpan? expireInterval = null);
        bool Contains(TValue value);
        bool ContainsAny(IEnumerable<TValue> values);
        bool ContainsAll(IEnumerable<TValue> values);
        TValue GetOrDefault(Func<TValue, bool> pred);
        List<TValue> GetRange(Func<TValue, bool> pred);
        TValue GetAndUpdateExpirationOrDefault(
            Func<TValue, bool> pred,
            TimeSpan? expireInterval = null);
        List<TValue> GetRangeAndUpdateExpiration(
            Func<TValue, bool> pred,
            TimeSpan? expireInterval = null);
        void Update(
            Func<TValue, bool> pred,
            TValue value,
            TimeSpan? expireInterval = null);
        void UpdateRange(
            Func<TValue, (bool, TValue)> pred,
            TimeSpan? expireInterval = null);
        void Delete(TValue value);
        void DeleteRange(Func<TValue, bool> pred);
        void Clear();
    }
    #endregion

    #region ILpInMemoryCache<TKey, TValue>
    public interface ILpInMemoryCache<TKey, TValue> : ILpInMemoryCache
        where TKey : notnull
    {
        void Add(TKey key, TValue value, TimeSpan? expireInterval = null);
        void AddRange(
            IEnumerable<KeyValuePair<TKey, TValue>> values,
            TimeSpan? expireInterval = null);
        bool Contains(TKey key);
        bool ContainsAny(IEnumerable<TKey> keys);
        bool ContainsAll(IEnumerable<TKey> keys);
        TValue Get(TKey key);
        TValue GetOrDefault(Func<TKey, TValue, bool> pred);
        List<TValue> GetRange(IEnumerable<TKey> keys);
        List<TValue> GetRange(Func<TKey, TValue, bool> pred);
        TValue GetAndUpdateExpiration(TKey key, TimeSpan? expireInterval = null);
        List<TValue> GetRangeAndUpdateExpiration(
            IEnumerable<TKey> keys,
            TimeSpan? expireInterval = null);
        void Update(TKey key, TValue value, TimeSpan? expireInterval = null);
        void UpdateRange(
            IEnumerable<KeyValuePair<TKey, TValue>> values,
            TimeSpan? expireInterval = null);
        void Delete(TKey key);
        void DeleteRange(IEnumerable<TKey> keys);
        void Clear();
    }
    #endregion
}
