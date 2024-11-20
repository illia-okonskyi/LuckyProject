using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public interface ILpPauseableWorker : ILpWorker
    {
        bool IsFinished { get; }
        bool IsPaused { get; }
        bool IsRunning { get; }
        int Progress { get; }
        Task PauseAsync();
        void Resume();
        Task<(bool Succeded, int ExitCode)> JoinUntilPausedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);
        Task<(bool Succeded, int ExitCode)> JoinUntilPausedOrStoppedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        delegate LpPauseableWorkerCancelSource GetCancelSourceFunc();
        delegate void SetProgressFunc(int progress);
    }

    public interface ILpPauseableWorker<TPayload>
        : ILpPauseableWorker
        , ILpWorker<TPayload>
        where TPayload : ILpWorkerPayload
    { }
}
