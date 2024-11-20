using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public abstract class AbstractJsonDocumentMigrator<TSource, TTarget>
         : IJsonDocumentMigrator
    {
        public int FromVersion { get; }
        public int ToVersion { get; }
        public Type ExpectedSourceType { get; }

        protected AbstractJsonDocumentMigrator(int fromVersion, int toVersion)
        {
            if (fromVersion < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(fromVersion));
            }

            if (toVersion <= fromVersion)
            {
                throw new ArgumentOutOfRangeException(nameof(toVersion));
            }

            FromVersion = fromVersion;
            ToVersion = toVersion;
            ExpectedSourceType = typeof(TSource);
        }

        public async Task<object> MigrateAsync(
            object source,
            CancellationToken cancellationToken = default)
        {
            if (!(source is TSource castedSource))
            {
                throw new ArgumentException(nameof(source));
            }

            return await MigrateAsyncImpl(castedSource, cancellationToken);
        }

        protected abstract Task<TTarget> MigrateAsyncImpl(
            TSource source,
            CancellationToken cancellationToken = default);
    }
}
