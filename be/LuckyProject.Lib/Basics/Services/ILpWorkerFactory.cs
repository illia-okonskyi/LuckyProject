using LuckyProject.Lib.Basics.LiveObjects.Workers;
using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpWorkerFactory
    {
        #region Payloads
        #region ForwardPayload
        LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpWorker<LpForwardWorkerPayload>,
                LpWorkerContext,
                Task<int>> callback);

        LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpPauseableWorker<LpForwardWorkerPayload>,
                LpPauseableWorkerContext,
                Task<int>> callback);

        LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpPauseableStateWorker<LpForwardWorkerPayload>,
                LpPauseableStateWorkerContext,
                Task<int>> callback);
        #endregion

        #region QueueRequestProcessorPayload
        LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>;

        LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpPauseableWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpPauseableWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>;

        LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpPauseableStateWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpPauseableStateWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>;
        #endregion
        #endregion

        #region Workers
        ILpWorker<TPayload> CreateWorker<TPayload>(
            TPayload payload,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload;
        ILpPauseableWorker<TPayload> CreatePauseableWorker<TPayload>(
            TPayload payload,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload;
        ILpPauseableStateWorker<TPayload> CreatePauseableStateWorker<TPayload>(
            TPayload payload,
            object initialState = default,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload;
        #endregion
    }
}
