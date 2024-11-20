using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    /// <summary>
    /// For internal usage only
    /// </summary>
    public abstract class AbstractLpWorkerBase : IDisposable
    {
        #region Internals
        private CancellationTokenSource stopCts;
        private Task task;
        #endregion

        #region Public properties
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; }
        public int ExitCode { get; protected set; } = ExitCodes.Success;
        #endregion

        #region Protected properties
        protected ILogger Logger { get; }
        protected bool IsStartedBase { get; private set; }
        #endregion

        #region ctor & Dispose
        protected AbstractLpWorkerBase(string name = null, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(name) ^ logger == null)
            {
                throw new ArgumentException(
                    "Name and logger must be both specified or not specified");
            }

            Name = name;
            Logger = logger;
        }

        public virtual void Dispose()
        {
            DisposeStopCts();
        }
        #endregion

        #region Protected interface
        protected void StartBase()
        {
            if (IsStartedBase)
            {
                return;
            }

            stopCts = new CancellationTokenSource();
            task = Task.Run(WorkerBaseAsync, stopCts.Token);
            IsStartedBase = true;
        }

        /// <summary>
        /// Returns true if not skipped
        /// </summary>
        protected async Task<bool> StopBaseAsync()
        {
            if (!IsStartedBase)
            {
                return false;
            }

            stopCts.Cancel();
            DisposeStopCts();
            await task;
            IsStartedBase = false;
            return true;
        }

        protected bool IsStopBaseRequested() => stopCts?.IsCancellationRequested ?? true;

        protected async Task<(bool Succeded, int ExitCode)> JoinBaseAsync(
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            if (!IsStartedBase)
            {
                return (true, ExitCodes.Success);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return (false, ExitCodes.Cancelled);
            }

            timeout ??= TimeoutDefaults.Infinity;
            using var timeoutCts = new CancellationTokenSource(timeout.Value);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                timeoutCts.Token,
                cancellationToken);
            try
            {
                await task;
                return (true, ExitCode);
            }
            catch (LpExitCodeException ex)
            {
                // NOTE: Succeded, just return received exit code
                return (true, ex.ExitCode);
            }
            catch (OperationCanceledException)
            {
                if (timeoutCts.IsCancellationRequested)
                {
                    return (false, ExitCodes.TimedOut);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return (false, ExitCodes.Cancelled);
                }

                // NOTE: Otherwise - swallow exceptions
                //       (they are already logged if loggin is enabled)
                return (false, ExitCodes.Unknown);
            }
            catch (Exception)
            {
                // NOTE: Swallow exceptions (they are already logged if loggin is enabled)
                return (false, ExitCodes.Unknown);
            }
        }

        protected async Task<(bool Succeded, int ExitCode)> JoinBaseAsync(
            Func<bool> pred,
            TimeSpan? singleCheckTimeout = null,
            TimeSpan? delayTimeout = null,
            TimeSpan? totalTimeout = null,
            CancellationToken cancellationToken = default)
        {
            singleCheckTimeout ??= TimeoutDefaults.Short;
            delayTimeout ??= TimeoutDefaults.Shortest;
            totalTimeout ??= TimeoutDefaults.Infinity;

            using var totalTimeoutCts = new CancellationTokenSource(totalTimeout.Value);
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                totalTimeoutCts.Token,
                cancellationToken);
            while (true)
            {
                (var success, var exitCode) = await JoinBaseAsync(
                    singleCheckTimeout,
                    combinedCts.Token);

                if (success && pred())
                {
                    return (true, exitCode);
                }

                if (exitCode == ExitCodes.Cancelled ||
                    !await TaskHelper.SafeDelay(delayTimeout.Value, combinedCts.Token))
                {
                    exitCode = totalTimeoutCts.IsCancellationRequested
                        ? ExitCodes.TimedOut
                        : ExitCodes.Cancelled;
                    return (false, exitCode);
                }
            }
        }
        #endregion

        #region Internals
        private async Task WorkerBaseAsync()
        {
            try
            {
                await WorkerAsyncImpl(stopCts.Token);
            }
            catch (OperationCanceledException ex)
            {
                if (IsStopBaseRequested())
                {
                    // Do nothing, swallow
                    return;
                }

                Logger?.LogError(ex, "Exception in Worker {Name}", Name);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Exception in Worker {Name}", Name);
            }
            finally
            {
                DisposeStopCts();
                IsStartedBase = false;
            }
        }

        private void DisposeStopCts()
        {
            stopCts?.Dispose();
            stopCts = null;
        }
        #endregion

        #region Abstract interface
        protected abstract Task WorkerAsyncImpl(CancellationToken cancellationToken);
        #endregion
    }
}
