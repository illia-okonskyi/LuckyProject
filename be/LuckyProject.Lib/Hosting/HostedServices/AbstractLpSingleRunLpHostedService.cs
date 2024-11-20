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
    public abstract class AbstractLpSingleRunLpHostedService
        : AbstractLpHostedServiceBase
        , ILpSingleRunLpHostedService
    {
        #region Internals
        private readonly CancellationTokenSource stopCts = new();
        private readonly ILpTimer timer;
        #endregion

        #region Protected properties
        protected ILpTimerFactory TimerFactory { get; }
        protected bool IsStopServiceRequested => stopCts.IsCancellationRequested;
        #endregion

        #region ctors
        /// <summary>
        /// Full ctor
        /// </summary>
        protected AbstractLpSingleRunLpHostedService(
            IServiceScopeService serviceScopeService,
            string name,
            ILogger logger,
            bool isPrimaryService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            bool isMainSingleService,
            ILpTimerFactory timerFactory,
            TimeSpan? execDelay)
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
            timer = TimerFactory.CreateTimer(execDelay.Value, true, OnTimerAsync);
        }

        /// <summary>
        /// Primary service
        /// </summary>
        protected AbstractLpSingleRunLpHostedService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
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
                  execDelay)
        { }

        /// <summary>
        /// Background service
        /// </summary>
        protected AbstractLpSingleRunLpHostedService(
            IServiceScopeService serviceScopeService,
            ILpTimerFactory timerFactory,
            string name,
            ILogger logger,
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
                  execDelay)
        { }

        /// <summary>
        /// Primary main single service
        /// </summary>
        protected AbstractLpSingleRunLpHostedService(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
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
                  execDelay)
        { }
        #endregion

        #region Protected virtual interface overrides
        protected override async Task StartServiceBaseAsync(CancellationToken cancellationToken)
        {
            await base.StartServiceBaseAsync(cancellationToken);
            await StartSingleRunServiceAsync(cancellationToken);
            timer.Start();
        }

        protected override async Task StopServiceBaseAsync(CancellationToken cancellationToken)
        {
            await base.StopServiceBaseAsync(cancellationToken);
            stopCts.Cancel();
            stopCts.Dispose();
            timer.Dispose();
            await StopSingleRunServiceAsync(cancellationToken);
        }
        #endregion

        #region Internals
        private async Task OnTimerAsync(DateTime signalTime, CancellationToken cancellationToken)
        {
            var execServiceLogScope = Logger?.BeginScope(
                "Executing {Name} service at {signalTime:o}",
                Name,
                signalTime);
            var exitCode = ExitCodes.Success;
            try
            {
                await ExecuteSingleRunServiceAsync(stopCts.Token);
            }
            catch (OperationCanceledException ex)
            {
                if (IsStopServiceRequested)
                {
                    exitCode = ExitCodes.Stopped;
                    Logger?.LogDebug("Stopped");
                    return;
                }

                Logger?.LogError(ex, "Exception");
                exitCode = ExitCodes.Unknown;
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
                LogRuntime(signalTime, exitCode);
                execServiceLogScope?.Dispose();
                if (IsMainSingleService)
                {
                    RequestStopApplication(exitCode);
                }
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
            Logger?.LogDebug(
                "Finised at {endTime:o}; ExitCode = {exitCode}; Runtime = {runtime:c}",
                endTime,
                exitCode.ExitCodeToString("c"),
                runtime);
        }
        #endregion

        #region Protected virtual/abstract interface
        protected virtual Task StartSingleRunServiceAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        protected virtual Task StopSingleRunServiceAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        protected virtual Task ExecuteSingleRunServiceAsync(
            CancellationToken cancellationToken) =>
            Task.Delay(TimeoutDefaults.Infinity, cancellationToken);
        #endregion
    }
}
