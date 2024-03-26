using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public interface ILpPausableWorker : ILpWorker
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

        delegate LpPausebleWorkerCancelSource GetCancelSourceFunc();
        delegate void SetProgressFunc(int progress);
        delegate void SetProgressStateFunc<TState>(int progress, TState state);
    }
}
