using LuckyProject.Lib.Basics.Constants;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public abstract class AbstractLpPauseableStateWorker<TState> : AbstractLpPauseableWorker
    {
        #region Internals
        private readonly TState initialState;
        #endregion

        #region Protected properties
        protected TState State { get; private set; }
        #endregion

        #region ctor
        protected AbstractLpPauseableStateWorker(
            TState initialState = default,
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        {
            this.initialState = initialState;
        }
        #endregion

        #region Protected interface
        protected void SetProgressState(int progress, TState state)
        {
            SetProgress(progress);
            State = state;
        }
        #endregion

        #region Internals
        protected override void Reset(bool newIsStarted)
        {
            State = initialState;
            base.Reset(newIsStarted);
        }

        protected override IDisposable CreateExecLoggerScope(DateTime startTime)
        {
            return Logger?.BeginScope(
                "Worker {Name} exec started at {startTime:o}; Progress = {Progress}%; State = {State}",
                Name,
                startTime,
                Progress,
                State);
        }

        protected override void LogRuntime(DateTime endTime, TimeSpan runtime, int exitCode)
        {
            Logger?.LogDebug(
                "Finised at {endTime:o}; ExitCode = {exitCode}; Runtime = {runtime:c}; Progress = {Progress}%; State = {State}",
                endTime,
                exitCode.ExitCodeToString("c"),
                runtime,
                Progress,
                State);
        }

        protected async override Task PausableWorkerAsync(
            int startingProgress,
            CancellationToken cancellationToken)
        {
            await PausableStateWorkerAsync(startingProgress, State, cancellationToken);
        }
        #endregion

        #region Abstract interface
        protected abstract Task PausableStateWorkerAsync(
            int startingProgress,
            TState startingState,
            CancellationToken cancellationToken);
        #endregion
    }
}
