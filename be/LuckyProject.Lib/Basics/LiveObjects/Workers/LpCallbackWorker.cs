using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpCallbackWorker : AbstractLpWorker
    {
        #region Internals
        private readonly Func<LpWorkerCallbackContext, Task> callback;
        #endregion

        #region ctor
        public LpCallbackWorker(
            Func<LpWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }
        #endregion

        #region Internals
        protected override Task WorkerAsync(CancellationToken cancellationToken) =>
            callback(new()
            {
                IsStopRequested = IsStopRequested,
                CancellationToken = cancellationToken
            });
        #endregion
    }
}
