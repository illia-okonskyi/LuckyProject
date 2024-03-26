using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPausableCallbackWorker : AbstractLpPauseableWorker
    {
        #region Internals
        private readonly Func<LpPausableWorkerCallbackContext, Task> callback;
        #endregion

        #region ctor
        public LpPausableCallbackWorker(
            Func<LpPausableWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }
        #endregion

        #region Internals
        protected override Task PausableWorkerAsync(
            int startingProgress,
            CancellationToken cancellationToken) =>
            callback(new()
            {
                GetCancelSource = GetCancelSource,
                SetProgress = SetProgress,
                StartingProgress = startingProgress,
                CancellationToken = cancellationToken
            });
        #endregion
    }
}
