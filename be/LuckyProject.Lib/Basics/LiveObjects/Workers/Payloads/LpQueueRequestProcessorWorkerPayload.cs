using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    public class LpQueueRequestProcessorWorkerPayload<TRequest, TQueue> : ILpWorkerPayload
        where TQueue : ILpQueueRequestProcessorQueue<TRequest>
    {
        #region Internals
        private readonly ManualResetEventSlim mres = new();
        private readonly object queueLock = new();
        private readonly IThreadSyncService tsService;

        private readonly Func<
            ILpWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
            LpWorkerContext,
            TRequest,
            Task<int>> workerCallback;
        private readonly Func<
            ILpPauseableWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
            LpPauseableWorkerContext,
            TRequest,
            Task<int>> pauseableWorkerCallback;
        private readonly Func<
            ILpPauseableStateWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
            LpPauseableStateWorkerContext,
            TRequest,
            Task<int>> pauseableStateWorkerCallback;
        #endregion

        #region Public properties
        public TQueue Queue { get; } = default;
        #endregion

        #region ctors & Dispose
        private LpQueueRequestProcessorWorkerPayload(IThreadSyncService tsService)
        {
            this.tsService = tsService;
        }

        public LpQueueRequestProcessorWorkerPayload(
            IThreadSyncService tsService,
             Func<
                ILpWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                LpWorkerContext,
                TRequest,
                Task<int>> callback)
            : this(tsService)
        {
            workerCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        public LpQueueRequestProcessorWorkerPayload(
            IThreadSyncService tsService,
            Func<
                ILpPauseableWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                LpPauseableWorkerContext,
                TRequest,
                Task<int>> callback)
            : this(tsService)
        {
            pauseableWorkerCallback = callback
                ?? throw new ArgumentNullException(nameof(callback));
        }

        public LpQueueRequestProcessorWorkerPayload(
            IThreadSyncService tsService,
            Func<
                ILpPauseableStateWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                LpPauseableStateWorkerContext,
                TRequest,
                Task<int>> callback)
            : this(tsService)
        {
            pauseableStateWorkerCallback = callback
                ?? throw new ArgumentNullException(nameof(callback));
        }

        public void Dispose()
        {
            mres.Dispose();
        }
        #endregion

        #region Public interface
        public Task<int> WorkerPayloadAsync<TPayload>(
            ILpWorker<TPayload> worker,
            LpWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            PayloadAsync(
                r => workerCallback(
                    worker as ILpWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    ctx,
                    r),
                ctx.CancellationToken);

        public Task<int> PauseableWorkerPayloadAsync<TPayload>(
            ILpPauseableWorker<TPayload> worker,
            LpPauseableWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            PayloadAsync(
                r => pauseableWorkerCallback(
                    worker as
                        ILpPauseableWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    ctx,
                    r),
                ctx.CancellationToken);

        public Task<int> PauseableStateWorkerPayloadAsync<TPayload>(
            ILpPauseableStateWorker<TPayload> worker,
            LpPauseableStateWorkerContext ctx)
            where TPayload : ILpWorkerPayload =>
            PayloadAsync(
                r => pauseableStateWorkerCallback(
                    worker as
                        ILpPauseableStateWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    ctx,
                    r),
                ctx.CancellationToken);
        #endregion

        #region Internals
        private async Task<int> PayloadAsync(
            Func<TRequest, Task<int>> processRequestAsync,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                if (!tsService.ManualResetEventSlimWait(mres, null, cancellationToken))
                {
                    return ExitCodes.Success;
                };

                mres.Reset();
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return ExitCodes.Success;
                    }

                    var (_, (isEmpty, request)) = tsService.MonitorGuard(
                        queueLock,
                        Queue.TryDequeueRequest);
                    if (isEmpty)
                    {
                        break;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return ExitCodes.Success;
                    }

                    var statusCode = await processRequestAsync(request);
                    if (statusCode == ExitCodes.Success || statusCode == ExitCodes.Rejected)
                    {
                        continue;
                    }

                    return statusCode;
                }
            }
        }
        #endregion
    }
}
