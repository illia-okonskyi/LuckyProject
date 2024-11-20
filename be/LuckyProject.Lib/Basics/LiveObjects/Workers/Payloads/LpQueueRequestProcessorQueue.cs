using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    public class LpQueueRequestProcessorQueue<TRequest>
        : Queue<TRequest>
        , ILpQueueRequestProcessorQueue<TRequest>
    {
        public (bool, TRequest) TryDequeueRequest() =>
            Count > 0 ? (false, Dequeue()) : (true, default);
    }
}
