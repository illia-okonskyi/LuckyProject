using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    public class LpForwardWorkerPayload : ILpWorkerPayload
    {
        #region Internals
        private readonly Func<
            ILpWorker<LpForwardWorkerPayload>,
            LpWorkerContext,
            Task<int>> workerCallback;
        private readonly Func<
            ILpPauseableWorker<LpForwardWorkerPayload>,
            LpPauseableWorkerContext,
            Task<int>> pauseableWorkerCallback;
        private readonly Func<
            ILpPauseableStateWorker<LpForwardWorkerPayload>,
            LpPauseableStateWorkerContext,
            Task<int>> pauseableStateWorkerCallback;
        #endregion

        #region ctors & Dispose
        public LpForwardWorkerPayload(
            Func<
                ILpWorker<LpForwardWorkerPayload>,
                LpWorkerContext,
                Task<int>> callback)
        {
            workerCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public LpForwardWorkerPayload(
            Func<
                ILpPauseableWorker<LpForwardWorkerPayload>,
                LpPauseableWorkerContext,
                Task<int>> callback)
        {
            pauseableWorkerCallback = callback
                ?? throw new ArgumentNullException(nameof(callback));
        }

        public LpForwardWorkerPayload(
            Func<
                ILpPauseableStateWorker<LpForwardWorkerPayload>,
                LpPauseableStateWorkerContext,
                Task<int>> callback)
        {
            pauseableStateWorkerCallback = callback
                ?? throw new ArgumentNullException(nameof(callback));
        }

        public void Dispose()
        { }
        #endregion

        #region Public interface
        public Task<int> WorkerPayloadAsync<TPayload>(
            ILpWorker<TPayload> worker,
            LpWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            workerCallback(
                worker as ILpWorker<LpForwardWorkerPayload>,
                ctx);

        public Task<int> PauseableWorkerPayloadAsync<TPayload>(
            ILpPauseableWorker<TPayload> worker,
            LpPauseableWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            pauseableWorkerCallback(
                worker as ILpPauseableWorker<LpForwardWorkerPayload>,
                ctx);

        public Task<int> PauseableStateWorkerPayloadAsync<TPayload>(
            ILpPauseableStateWorker<TPayload> worker,
            LpPauseableStateWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            pauseableStateWorkerCallback(
                worker as ILpPauseableStateWorker<LpForwardWorkerPayload>,
                ctx);
        #endregion
    }
}
