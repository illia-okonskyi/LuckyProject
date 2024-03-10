using LuckyProject.Lib.Basics.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToEmptyIfNull<T>(this IEnumerable<T> e)
        {
            return e == null ? Enumerable.Empty<T>() : e;
        }

        public static int SafeCount<T>(this IEnumerable<T> e)
        {
            return e.TryGetNonEnumeratedCount(out var count) ? count : e.Count();
        }

        public static IEnumerable<T> ToNullIfEmpty<T>(this IEnumerable<T> e)
        {
            return e.SafeCount() > 0 ? e : null;
        }

        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> e,
            int pageSize,
            int page,
            bool isCyclic = false)
        {
            return new PaginatedList<T>
            {
                Pagination = new PaginationMetadata(e.SafeCount(), pageSize, page, isCyclic),
                Items = e.Skip(pageSize * (page - 1)).Take(pageSize).ToList()
            };
        }
    }
}
