using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public interface ILpPauseableStateWorker : ILpPauseableWorker
    {
        delegate void SetProgressStateFunc(int progress, object state);
    }

    public interface ILpPauseableStateWorker<TPayload>
        : ILpPauseableStateWorker
        , ILpWorker<TPayload>
        where TPayload : ILpWorkerPayload
    { }
}
