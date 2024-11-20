using LuckyProject.Lib.Basics.Collections;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    public class LpQueueRequestProcessorPriorityQueue<TRequest>
        : LpPriorityQueue<TRequest>
        , ILpQueueRequestProcessorQueue<TRequest>
    {
        public (bool, TRequest) TryDequeueRequest() =>
            TotalCount > 0 ? (false, Dequeue()) : (true, default);
    }
}
