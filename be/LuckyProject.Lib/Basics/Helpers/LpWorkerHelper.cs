using LuckyProject.Lib.Basics.LiveObjects.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Helpers
{
    public static class LpWorkerHelper
    {
        #region WhenAll
        public static async Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilStoppedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            return (await Task.WhenAll(tasks))
                .Select(t => (t.id, t.r.Succeded, t.r.ExitCode))
                .ToList();
        }

        public static async Task<List<(Guid Id, bool Succeded, int ExitCode)>> WhenAllPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilPausedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            return (await Task.WhenAll(tasks))
                .Select(t => (t.id, t.r.Succeded, t.r.ExitCode))
                .ToList();
        }

        public static async Task<List<(Guid Id, bool Succeded, int ExitCode)>>
            WhenAllPausedOrStoppedAsync(
                IEnumerable<ILpPauseableWorker> workers,
                TimeSpan? timeout = null,
                CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilPausedOrStoppedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            return (await Task.WhenAll(tasks))
                .Select(t => (t.id, t.r.Succeded, t.r.ExitCode))
                .ToList();
        }
        #endregion

        #region WhenAny
        public static async Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyStoppedAsync(
            IEnumerable<ILpWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilStoppedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            var task = await Task.WhenAny(tasks);
            (var id, (var succeded, var exitCode)) = await task;
            return (id, succeded, exitCode);
        }

        public static async Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilPausedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            var task = await Task.WhenAny(tasks);
            (var id, (var succeded, var exitCode)) = await task;
            return (id, succeded, exitCode);
        }

        public static async Task<(Guid Id, bool Succeded, int ExitCode)> WhenAnyPausedOrStoppedAsync(
            IEnumerable<ILpPauseableWorker> workers,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            var tasks = workers
                .Select(async w =>
                {
                    var result = await w.JoinUntilPausedOrStoppedAsync(timeout, cancellationToken);
                    return (id: w.Id, r: result);
                });
            var task = await Task.WhenAny(tasks);
            (var id, (var succeded, var exitCode)) = await task;
            return (id, succeded, exitCode);
        }
        #endregion
    }
}
