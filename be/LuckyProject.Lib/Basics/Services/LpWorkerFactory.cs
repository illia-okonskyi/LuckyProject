using LuckyProject.Lib.Basics.LiveObjects.Workers;
using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpWorkerFactory : ILpWorkerFactory
    {
        #region Internals
        private readonly IThreadSyncService tsService;
        #endregion

        #region ctor
        public LpWorkerFactory(IThreadSyncService tsService)
        {
            this.tsService = tsService;
        }
        #endregion

        #region Payloads
        #region ForwardPayload
        public LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpWorker<LpForwardWorkerPayload>,
                LpWorkerContext,
                Task<int>> callback)
        {
            return new LpForwardWorkerPayload(callback);
        }

        public LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpPauseableWorker<LpForwardWorkerPayload>,
                LpPauseableWorkerContext,
                Task<int>> callback)
        {
            return new LpForwardWorkerPayload(callback);
        }

        public LpForwardWorkerPayload CreateForwardPayload(
            Func<
                ILpPauseableStateWorker<LpForwardWorkerPayload>,
                LpPauseableStateWorkerContext,
                Task<int>> callback)
        {
            return new LpForwardWorkerPayload(callback);
        }
        #endregion

        #region QueueRequestProcessorPayload
        public LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>
        {
            return new LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>(tsService, callback);
        }

        public LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpPauseableWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpPauseableWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>
        {
            return new LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>(tsService, callback);
        }

        public LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>
            CreateQueueRequestProcessorPayload<TRequest, TQueue>(
                Func<
                    ILpPauseableStateWorker<LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>>,
                    LpPauseableStateWorkerContext,
                    TRequest,
                    Task<int>> callback)
            where TQueue : ILpQueueRequestProcessorQueue<TRequest>
        {
            return new LpQueueRequestProcessorWorkerPayload<TRequest, TQueue>(tsService, callback);
        }
        #endregion
        #endregion

        #region Workers
        public ILpWorker<TPayload> CreateWorker<TPayload>(
            TPayload payload,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload
        {
            return new LpWorker<TPayload>(payload, name, logger);
        }

        public ILpPauseableWorker<TPayload> CreatePauseableWorker<TPayload>(
            TPayload payload,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload
        {
            return new LpPauseableWorker<TPayload>(payload, name, logger);
        }

        public ILpPauseableStateWorker<TPayload> CreatePauseableStateWorker<TPayload>(
            TPayload payload,
            object initialState = default,
            string name = null,
            ILogger logger = null)
            where TPayload : ILpWorkerPayload
        {
            return new LpPauseableStateWorker<TPayload>(payload, initialState, name, logger);
        }
        #endregion
    }
}
