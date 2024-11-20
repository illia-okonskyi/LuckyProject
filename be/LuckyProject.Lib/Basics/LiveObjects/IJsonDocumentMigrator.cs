using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public interface IJsonDocumentMigrator
    {
        int FromVersion { get; }
        int ToVersion { get; }
        Type ExpectedSourceType { get; }

        Task<object> MigrateAsync(object source, CancellationToken cancellationToken = default);
    }
}
