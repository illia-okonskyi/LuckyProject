using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public abstract class AbstractLpWorker : AbstractLpWorkerBase, ILpWorker
    {
        #region Public  & Protected properties
        public bool IsStarted => IsStartedBase;
        #endregion

        #region ctor
        protected AbstractLpWorker(string name = null, ILogger logger = null)
            : base(name, logger)
        { }
        #endregion

        #region Public interface
        public void Start() => StartBase();

        public async Task StopAsync()
        {
            if (!await StopBaseAsync())
            {
                return;
            }
        }

        public Task<(bool Succeded, int ExitCode)> JoinUntilStoppedAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default) =>
            JoinBaseAsync(timeout, cancellationToken);
        #endregion

        #region Protected inteface
        protected bool IsStopRequested() => IsStopBaseRequested();
        #endregion

        #region Internals
        protected async override Task WorkerAsyncImpl(CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            var execLogScope = Logger?.BeginScope(
                "Worker {Name} exec started at {startTime:o}",
                Name,
                startTime);

            var exitCode = ExitCodes.Success;
            try
            {
                await WorkerAsync(cancellationToken);
            }
            catch (OperationCanceledException ex)
            {
                if (IsStopBaseRequested())
                {
                    exitCode = ExitCodes.Cancelled;
                    Logger?.LogDebug("Cancelled");
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
                LogRuntime(startTime, exitCode);
                execLogScope?.Dispose();
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
            Logger?.LogDebug(
                "Finised at {endTime:o}; ExitCode = {exitCode}; Runtime = {runtime:c}",
                endTime,
                exitCode.ExitCodeToString("c"),
                runtime);
        }
        #endregion

        #region Abstract interface
        protected abstract Task WorkerAsync(CancellationToken cancellationToken);
        #endregion
    }
}
