using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.LiveObjects.Workers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpWorkerService : LpWorkerFactory, ILpWorkerService
    {
        #region ctor
        public LpWorkerService(IThreadSyncService tsService) : base(tsService)
        { }
        #endregion

        #region WhenAll
        public Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAllStoppedAsync(workers, timeout, cancellationToken);

        public Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAllPausedAsync(workers, timeout, cancellationToken);

        public Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllPausedOrStoppedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAllPausedOrStoppedAsync(workers, timeout, cancellationToken);
        #endregion

        #region WhenAny
        public Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAnyStoppedAsync(workers, timeout, cancellationToken);

        public Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAnyPausedAsync(workers, timeout, cancellationToken);

        public Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedOrStoppedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            LpWorkerHelper.WhenAnyPausedOrStoppedAsync(workers, timeout, cancellationToken);
        #endregion
    }
}
