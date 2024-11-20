using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public interface ILpWorker : IDisposable
    {
        Guid Id { get; }
        string Name { get; }
        int ExitCode { get; }
        bool IsStarted { get; }
        void Start();
        Task StopAsync();
        Task<(bool Succeded, int ExitCode)> JoinUntilStoppedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default);

        delegate bool IsStopRequestedFunc();
    }

    public interface ILpWorker<TPayload>
        where TPayload : ILpWorkerPayload
    {
        TPayload Payload { get; }
    }
}
