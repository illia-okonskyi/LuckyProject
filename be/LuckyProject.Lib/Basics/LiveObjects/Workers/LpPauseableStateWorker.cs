using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPauseableStateWorker<TPayload>
        : AbstractLpPauseableWorkerBase
        , ILpPauseableStateWorker<TPayload>
        where TPayload : ILpWorkerPayload
    {
        #region Internals
        private readonly object initialState;
        private object state;
        #endregion

        #region Public properties
        public TPayload Payload { get; }
        #endregion

        #region ctor & Dispose
        public LpPauseableStateWorker(
            TPayload payload,
            object initialState = default,
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            this.initialState = initialState;
        }

        public override void Dispose()
        {
            Payload.Dispose();
            base.Dispose();
        }
        #endregion

        #region Internals
        private void SetProgressState(int progress, object state)
        {
            SetProgress(progress);
            this.state = state;
        }

        protected override void Reset(bool newIsStarted)
        {
            state = initialState;
            base.Reset(newIsStarted);
        }

        protected override IDisposable CreateExecLoggerScope(DateTime startTime)
        {
            return Logger?.BeginScope(
                "Worker {Name} exec started at {startTime:o}; Progress = {Progress}%; State = {State}",
                Name,
                startTime,
                Progress,
                state);
        }

        protected override void LogRuntime(DateTime endTime, TimeSpan runtime, int exitCode)
        {
            Logger?.LogDebug(
                "Finised at {endTime:o}; ExitCode = {exitCode}; Runtime = {runtime:c}; Progress = {Progress}%; State = {State}",
                endTime,
                exitCode.ExitCodeToString("c"),
                runtime,
                Progress,
                state);
        }

        protected override Task<int> PauseableWorkerAsync(
            int startingProgress,
            CancellationToken cancellationToken) =>
            Payload.PauseableStateWorkerPayloadAsync(
                this,
                new()
                {
                    StartingProgress = startingProgress,
                    StartingState = state,
                    CancellationToken = cancellationToken,
                    SetProgress = SetProgress,
                    SetProgressState = SetProgressState,
                    GetCancelSource = GetCancelSource
                });
        #endregion
    }
}
