using LuckyProject.Lib.Basics.Collections;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> e)
        {
            return e == null || e.SafeCount() == 0;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> e)
        {
            return e == null ? Enumerable.Empty<T>() : e;
        }

        public static int SafeCount<T>(this IEnumerable<T> e)
        {
            return e.TryGetNonEnumeratedCount(out var count) ? count : e.Count();
        }

        public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> e)
        {
            return e.SafeCount() > 0 ? e : null;
        }

        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> e,
            int pageSize,
            int page,
            bool isCyclic = false)
        {
            var pagination = new PaginationMetadata(e.SafeCount(), pageSize, page, isCyclic);
            return new PaginatedList<T>
            {
                Pagination = pagination,
                Items = e
                    .Skip(pagination.PageSize * (pagination.Page - 1))
                    .Take(pagination.PageSize)
                    .ToList()
            };
        }

        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> e,
            PaginationMetadata pagination)
        {
            return new PaginatedList<T>
            {
                Pagination = pagination,
                Items = e.ToList()
            };
        }

        public static StringValues ToStringValues(this IEnumerable<string> e)
        {
            return new StringValues(e?.ToArray() ?? []);
        }

        public static async Task<List<T>> ToListAsync<T>(
            this IAsyncEnumerable<T> ae,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(ae);

            var result = new List<T>();
            await foreach (var item in ae)
            {
                ct.ThrowIfCancellationRequested();
                result.Add(item);
            }

            return result;
        }
    }
}
