using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public abstract class AbstractLpPauseableWorkerBase : AbstractLpWorkerBase
    {
        #region Internals
        private CancellationTokenSource pauseCts;
        #endregion

        #region Public properties
        public bool IsStarted { get; private set; }
        public bool IsFinished => !IsRunning && Progress == CommonConstants.ProgressMax;
        public bool IsPaused { get; private set; }
        public bool IsRunning => IsStarted && !IsPaused;
        public int Progress { get; private set; } = CommonConstants.ProgressMin;
        #endregion

        #region ctor & Dispose
        public AbstractLpPauseableWorkerBase(
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        { }

        public override void Dispose()
        {
            DisposePauseCts();
            base.Dispose();
        }

        #endregion

        #region Public interface
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            if (IsPaused)
            {
                throw new InvalidOperationException(
                    $"Worker {Name} is paused. Stop Worker before restarting");
            }

            pauseCts = new CancellationTokenSource();
            StartBase();
            Reset(true);
        }

        public async Task StopAsync()
        {
            if (!IsStarted)
            {
                return;
            }

            if (IsPaused)
            {
                Reset(false);
                return;
            }

            await StopBaseAsync();
            Reset(false);
        }

        public async Task PauseAsync()
        {
            PauseResumeGuard();

            if (IsPaused)
            {
                return;
            }

            pauseCts.Cancel();
            DisposePauseCts();
            await StopBaseAsync();
            IsPaused = true;
        }

        public void Resume()
        {
            PauseResumeGuard();

            if (!IsPaused)
            {
                return;
            }

            pauseCts = new CancellationTokenSource();
            StartBase();
            IsPaused = false;
        }

        public Task<(bool Succeded, int ExitCode)> JoinUntilStoppedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            JoinBaseAsync(
                () => IsFinished,
                totalTimeout: timeout,
                cancellationToken: cancellationToken);

        public async Task<(bool Succeded, int ExitCode)> JoinUntilPausedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            (var succeded, var exitCode) = await JoinUntilPausedOrStoppedAsync(
                timeout,
                cancellationToken);
            return (succeded && IsPaused, exitCode);
        }

        public Task<(bool Succeded, int ExitCode)>
            JoinUntilPausedOrStoppedAsync(
                TimeSpan? timeout = null,
                CancellationToken cancellationToken = default) =>
            JoinBaseAsync(timeout, cancellationToken);
        #endregion

        #region Protected interface
        protected virtual void Reset(bool newIsStarted)
        {
            if (!newIsStarted)
            {
                DisposePauseCts();
            }

            Progress = CommonConstants.ProgressMin;
            IsStarted = newIsStarted;
            IsPaused = false;
        }

        protected LpPauseableWorkerCancelSource GetCancelSource()
        {
            if (IsStopBaseRequested() && pauseCts != null)
            {
                return pauseCts.IsCancellationRequested
                    ? LpPauseableWorkerCancelSource.Pause
                    : LpPauseableWorkerCancelSource.Stop;
            }

            return LpPauseableWorkerCancelSource.Other;
        }

        protected void SetProgress(int progress)
        {
            if (!CommonConstants.ProgressInRange(progress))
            {
                throw new ArgumentOutOfRangeException(nameof(progress));
            }

            Progress = progress;
        }

        protected virtual IDisposable CreateExecLoggerScope(DateTime startTime)
        {
            return Logger?.BeginScope(
                "Worker {Name} exec started at {startTime:o}; Progress = {Progress}%",
                Name,
                startTime,
                Progress);
        }

        protected virtual void LogRuntime(DateTime endTime, TimeSpan runtime, int exitCode)
        {
            Logger?.LogDebug(
                "Finised at {endTime:o}; ExitCode = {exitCode}; Runtime = {runtime:c}; Progress = {Progress}%",
                endTime,
                exitCode.ExitCodeToString("c"),
                runtime,
                Progress);
        }
        #endregion

        #region Internals
        private void PauseResumeGuard()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException(
                    $"Worker {Name} is not started. Start Worker before pausing");
            }

            if (IsFinished)
            {
                throw new InvalidOperationException("Worker {Name} finished. Restart Worker");
            }
        }

        protected async override Task WorkerAsyncImpl(CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            var execLogScope = CreateExecLoggerScope(startTime);

            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                pauseCts.Token);

            var exitCode = ExitCodes.Success;
            try
            {
                exitCode = await PauseableWorkerAsync(Progress, combinedCts.Token);
                SetProgress(CommonConstants.ProgressMax);
                IsStarted = false;
            }
            catch (OperationCanceledException ex)
            {
                var cancelSource = GetCancelSource();
                if (cancelSource == LpPauseableWorkerCancelSource.Stop)
                {
                    exitCode = ExitCodes.Stopped;
                    Logger?.LogDebug("Stopped");
                    return;
                }
                else if (cancelSource == LpPauseableWorkerCancelSource.Pause)
                {
                    exitCode = ExitCodes.Paused;
                    Logger?.LogDebug("Paused");
                    return;
                }

                exitCode = ExitCodes.Unknown;
                Logger?.LogError(ex, "Exception");
            }
            catch (LpExitCodeException ex)
            {
                Logger?.LogDebug(ex, "Exit code received");
                exitCode = ex.ExitCode;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Exception");
                exitCode = ExitCodes.Unknown;
            }
            finally
            {
                LogRuntime(startTime, exitCode);
                execLogScope?.Dispose();
                DisposePauseCts();
                ExitCode = exitCode;
            }
        }

        private void LogRuntime(DateTime startTime, int exitCode)
        {
            if (Logger == null)
            {
                return;
            }

            var endTime = DateTime.Now;
            var runtime = endTime - startTime;
            LogRuntime(endTime, runtime, exitCode);
        }

        private void DisposePauseCts()
        {
            pauseCts?.Dispose();
            pauseCts = null;
        }
        #endregion

        #region Abstract interface
        protected abstract Task<int> PauseableWorkerAsync(
            int startingProgress,
            CancellationToken cancellationToken);
        #endregion
    }
}
