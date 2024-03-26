using System.Threading;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPausableWorkerCallbackContext
    {
        public ILpPausableWorker.GetCancelSourceFunc GetCancelSource { get; init; }
        public ILpPausableWorker.SetProgressFunc SetProgress { get; init; }
        public int StartingProgress { get; init; }
        public CancellationToken CancellationToken { get; init; }
    }
}
