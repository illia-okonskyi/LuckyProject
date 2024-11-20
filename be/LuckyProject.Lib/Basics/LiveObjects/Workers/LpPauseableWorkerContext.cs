using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using System.Threading;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPauseableWorkerContext
    {
        public ILpPauseableWorker.GetCancelSourceFunc GetCancelSource { get; init; }
        public ILpPauseableWorker.SetProgressFunc SetProgress { get; init; }
        public int StartingProgress { get; init; }
        public CancellationToken CancellationToken { get; init; }
    }
}
