using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPauseableStateWorkerContext : LpPauseableWorkerContext
    {
        public ILpPauseableStateWorker.SetProgressStateFunc SetProgressState { get; init; }
        public object StartingState { get; init; }
    }
}
