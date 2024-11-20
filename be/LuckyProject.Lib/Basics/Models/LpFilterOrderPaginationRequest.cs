using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Models
{
    public class LpFilterOrderPaginationRequest
    {
        public Dictionary<string, object> Filters { get; init; } = new();
        public string Order { get; init; }
        public int Page { get; init; } = 1;

        public T GetFilter<T>(string key)
        {
            if (!Filters.TryGetValue(key, out var value))
            {
                return default;
            }

            return value is T casted ? casted : default;
        }

        public TEnum? GetEnumFilter<TEnum>(string key)
            where TEnum : struct, Enum
        {
            var s = GetFilter<string>(key);
            if (string.IsNullOrEmpty(s))
            {
                return default;
            }

            return Enum.TryParse<TEnum>(s, out var value) ? value : default;
        }
    }
}
