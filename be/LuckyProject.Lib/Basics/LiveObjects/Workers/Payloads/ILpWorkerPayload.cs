using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    /// <summary>
    /// Visitor pattern
    /// </summary>
    public interface ILpWorkerPayload : IDisposable
    {
        Task<int> WorkerPayloadAsync<TPayload>(
            ILpWorker<TPayload> worker,
            LpWorkerContext ctx)
            where TPayload : ILpWorkerPayload;
        Task<int> PauseableWorkerPayloadAsync<TPayload>(
            ILpPauseableWorker<TPayload> worker,
            LpPauseableWorkerContext ctx)
            where TPayload : ILpWorkerPayload;
        Task<int> PauseableStateWorkerPayloadAsync<TPayload>(
            ILpPauseableStateWorker<TPayload> worker,
            LpPauseableStateWorkerContext ctx)
            where TPayload : ILpWorkerPayload;
    }
}
