using LuckyProject.Lib.Basics.Collections;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class EfCoreExtensions
    {
        #region ToPaginatedListAsync
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> q,
            int pageSize,
            int page,
            bool isCyclic = false,
            CancellationToken cancellationToken = default)
        {
            if (pageSize < 1)
            {
                throw new ArgumentException(nameof(pageSize));
            }

            if (page < 1)
            {
                throw new ArgumentException(nameof(page));
            }

            var totalItemsCount = await q.CountAsync(cancellationToken);
            var pagination = new PaginationMetadata(totalItemsCount, pageSize, page, isCyclic);
            var items = await q
                .Skip(pagination.PageSize * (pagination.Page - 1))
                .Take(pagination.PageSize)
                .ToListAsync(cancellationToken);
            return new PaginatedList<T>
            {
                Pagination = pagination,
                Items = items
            };
        }

        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> q,
            PaginationMetadata pagination,
            CancellationToken cancellationToken = default)
        {
            return new PaginatedList<T>
            {
                Pagination = pagination,
                Items = await q.ToListAsync(cancellationToken)
            };
        }
        #endregion

        #region BeginTransactionIfNoCurrent
        public static IDbContextTransaction BeginTransactionIfNoCurrent(this DatabaseFacade db)
        {
            if (db.CurrentTransaction != null)
            {
                return null;
            }

            return db.BeginTransaction();
        }

        public static async Task<IDbContextTransaction> BeginTransactionIfNoCurrentAsync(
            this DatabaseFacade db,
            CancellationToken cancellationToken = default)
        {
            if (db.CurrentTransaction != null)
            {
                return null;
            }

            return await db.BeginTransactionAsync(cancellationToken);
        }
        #endregion

        #region Transaction action or skip if null
        public static async Task CommitOrSkipIfNullAsync(
            this IDbContextTransaction t,
            CancellationToken cancellationToken = default)
        {
            if (t == null)
            {
                return;
            }

            await t.CommitAsync(cancellationToken);
        }

        public static async Task RollbackOrSkipIfNullAsync(
            this IDbContextTransaction t,
            CancellationToken cancellationToken = default)
        {
            if (t == null)
            {
                return;
            }

            await t.RollbackAsync(cancellationToken);
        }

        public static async Task CreateSavepointOrSkipIfNullAsync(
            this IDbContextTransaction t,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (t == null)
            {
                return;
            }

            await t.CreateSavepointAsync(name, cancellationToken);
        }

        public static async Task RollbackToSavepointOrSkipIfNullAsync(
            this IDbContextTransaction t,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (t == null)
            {
                return;
            }

            await t.RollbackToSavepointAsync(name, cancellationToken);
        }

        public static async Task ReleaseSavepointOrSkipIfNullAsync(
            this IDbContextTransaction t,
            string name,
            CancellationToken cancellationToken = default)
        {
            if (t == null)
            {
                return;
            }

            await t.ReleaseSavepointAsync(name, cancellationToken);
        }
        #endregion
    }
}
