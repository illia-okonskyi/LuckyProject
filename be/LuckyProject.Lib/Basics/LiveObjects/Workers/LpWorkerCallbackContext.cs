using System.Threading;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpWorkerCallbackContext
    {
        public ILpWorker.IsStopRequestedFunc IsStopRequested { get; init; }
        public CancellationToken CancellationToken { get; init; }
    }
}
