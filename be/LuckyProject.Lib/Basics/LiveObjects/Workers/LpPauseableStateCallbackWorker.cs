using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPauseableStateCallbackWorker<TState> : AbstractLpPauseableStateWorker<TState>
    {
        #region Internals
        private readonly Func<LpPausableStateWorkerCallbackContext<TState>, Task> callback;
        #endregion

        #region ctor
        public LpPauseableStateCallbackWorker(
            Func<LpPausableStateWorkerCallbackContext<TState>, Task> callback,
            TState initialState = default,
            string name = null,
            ILogger logger = null)
            : base(initialState, name, logger)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }
        #endregion

        #region Internals
        protected override Task PausableStateWorkerAsync(
            int startingProgress,
            TState startingState,
            CancellationToken cancellationToken) =>
            callback(new()
            {
                GetCancelSource = GetCancelSource,
                SetProgress = SetProgress,
                SetProgressState = SetProgressState,
                StartingProgress = startingProgress,
                StartingState = startingState,
                CancellationToken = cancellationToken
            });
        #endregion
    }
}
