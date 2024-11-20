namespace LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads
{
    public interface ILpQueueRequestProcessorQueue<TRequest>
    {
        (bool, TRequest) TryDequeueRequest();
    }
}
