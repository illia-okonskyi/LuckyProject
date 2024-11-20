using LuckyProject.Lib.Basics.LiveObjects.Workers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpWorkerService : ILpWorkerFactory
    {
        #region WhenAll
        Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllPausedOrStoppedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);
        #endregion

        #region WhenAny
        Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedOrStoppedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
