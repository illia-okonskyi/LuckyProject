using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using System;
using System.Threading;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpWorkerContext
    {
        public ILpWorker.IsStopRequestedFunc IsStopRequested { get; init; }
        public CancellationToken CancellationToken { get; init; }
    }
}
