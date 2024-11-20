using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.LiveObjects;
using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.HostedServices
{
    public abstract class AbstractLpPeriodicLpHostedService
        : AbstractLpHostedServiceBase
        , ILpPeriodicHostedService
    {
        #region Internals
        private CancellationTokenSource firstRunStopCts = new();
        private ILpTimer firstRunTimer;

        private readonly ILpTimer nextRunsTimer;

        private CancellationToken? currentCt;
        #endregion

        #region Public & protected properties

        protected ILpTimerFactory TimerFactory { get; }
        public TimeSpan Interval { get; }
        public bool IsExecuting { get; private set; }

        protected bool IsCurrentExecuteStopRequested => currentCt?.IsCancellationRequested ?? false;
        #endregion

        #region ctors
        /// <summary>
        /// Full ctor
        /// </summary>
        protected AbstractLpPeriodicLpHostedService(
            IServiceScopeService serviceScopeService,
            string name,
            ILogger logger,
            bool isPrimaryService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            bool isMainSingleService,
            ILpTimerFactory timerFactory,
            TimeSpan? execDelay,
            TimeSpan interval)
            : base(
                  serviceScopeService,
                  name,
                  logger,
                  isPrimaryService,
                  appLifetime,
                  environmentService,
                  isMainSingleService)
        {
            execDelay ??= TimeoutDefaults.AlmostZero;

            TimerFactory = timerFactory;
            firstRunTimer = TimerFactory.CreateTimer(
                execDelay.Value,
                true,
                OnFirstRunTimerAsync);
            nextRunsTimer = TimerFactory.CreateTimer(
                interval,
                false,
                OnNextRunsTimerAsync,
                true);
        }

        /// <summary>
        /// Primary service
        /// </summary>
        protected AbstractLpPeriodicLpHostedService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            TimeSpan interval,
            string name,
            ILogger logger = null,
            TimeSpan? execDelay = null)
            : this(
                  serviceScopeService,
                  name,
                  logger,
                  true,
                  appLifetime,
                  environmentService,
                  false,
                  timerFactory,
                  execDelay,
                  interval)
        { }

        /// <summary>
        /// Background service
        /// </summary>
        protected AbstractLpPeriodicLpHostedService(
            IServiceScopeService serviceScopeService,
            ILpTimerFactory timerFactory,
            TimeSpan interval,
            string name,
            ILogger logger = null,
            TimeSpan? execDelay = null)
            : this(
                  serviceScopeService,
                  name,
                  logger,
                  false,
                  null,
                  null,
                  false,
                  timerFactory,
                  execDelay,
                  interval)
        { }

        /// <summary>
        /// Primary main single service
        /// </summary>
        protected AbstractLpPeriodicLpHostedService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            TimeSpan interval,
            ILogger logger = null,
            TimeSpan? execDelay = null)
            : this(
                  serviceScopeService,
                  MainServiceName,
                  logger,
                  true,
                  appLifetime,
                  environmentService,
                  true,
                  timerFactory,
                  execDelay,
                  interval)
        { }
        #endregion

        #region Protected virtual interface overrides
        protected override async Task StartServiceBaseAsync(CancellationToken cancellationToken)
        {
            await base.StartServiceBaseAsync(cancellationToken);
            await StartPeriodicServiceAsync(cancellationToken);
            firstRunTimer.Start();
        }

        protected override async Task StopServiceBaseAsync(CancellationToken cancellationToken)
        {
            await base.StopServiceBaseAsync(cancellationToken);
            ClearFirstRun();
            nextRunsTimer.Stop();
            nextRunsTimer.Dispose();
            await StopPeriodicServiceAsync(cancellationToken);
        }
        #endregion

        #region Internals
        private void ClearFirstRun()
        {
            if (firstRunStopCts != null)
            {
                firstRunStopCts.Cancel();
                firstRunStopCts.Dispose();
                firstRunStopCts = null;
            }

            if (firstRunTimer != null)
            {
                firstRunTimer?.Dispose();
                firstRunTimer = null;
            }
        }

        private async Task OnFirstRunTimerAsync(
            DateTime signalTime,
            CancellationToken cancellationToken)
        {
            currentCt = firstRunStopCts.Token;
            await OnTimerAsync(true, signalTime, currentCt.Value);
            ClearFirstRun();
        }

        private async Task OnNextRunsTimerAsync(
            DateTime signalTime,
            CancellationToken cancellationToken)
        {
            currentCt = cancellationToken;
            await OnTimerAsync(false, signalTime, cancellationToken);
        }

        private async Task OnTimerAsync(
            bool isFirstRun,
            DateTime signalTime,
            CancellationToken cancellationToken)
        {
            var execServiceLogScope = Logger?.BeginScope(
                "Executing {Name} service at {signalTime:o}; IsFirstRun = {isFirstRun}",
                Name,
                signalTime,
                isFirstRun);
            var localExitCode = ExitCodes.Success;
            try
            {
                IsExecuting = true;
                await ExecutePeriodicServiceAsync(isFirstRun, cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                if (IsCurrentExecuteStopRequested)
                {
                    localExitCode = ExitCodes.Stopped;
                    Logger?.LogDebug("Stopped");
                    return;
                }

                Logger?.LogError(ex, "Exception");
                localExitCode = ExitCodes.Unknown;
            }
            catch (LpExitCodeException ex)
            {
                Logger?.LogDebug(ex, "Local exit code received");
                localExitCode = ex.ExitCode;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Exception");
                localExitCode = ExitCodes.Unknown;
            }
            finally
            {
                LogRuntime(signalTime, localExitCode);
                execServiceLogScope?.Dispose();
                IsExecuting = false;

                if (isFirstRun)
                {
                    nextRunsTimer.Start();
                }
            }
        }

        private void LogRuntime(DateTime startTime, int localExitCode)
        {
            if (Logger == null)
            {
                return;
            }

            var endTime = DateTime.Now;
            var runtime = endTime - startTime;
            Logger?.LogDebug(
                "Finised at {endTime:o}; LocalExitCode = {localExitCode}; Runtime = {runtime:c}",
                endTime,
                localExitCode.ExitCodeToString("c"),
                runtime);
        }
        #endregion

        #region Protected virtual/abstract interface
        protected virtual Task StartPeriodicServiceAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        protected virtual Task StopPeriodicServiceAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        protected abstract Task ExecutePeriodicServiceAsync(
            bool isFirstRun,
            CancellationToken cancellationToken);
        #endregion
    }
}
