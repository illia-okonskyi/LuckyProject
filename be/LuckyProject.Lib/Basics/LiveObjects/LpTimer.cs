using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public class LpTimer : ILpTimer
    {
        #region Internals
        private readonly System.Timers.Timer backend;
        private readonly Action<DateTime, CancellationToken> callback;
        private readonly Func<DateTime, CancellationToken, Task> asyncCallback;
        private readonly ILogger logger;
        private CancellationTokenSource stopCts;
        #endregion

        #region Public properties
        public string Name { get; }
        public TimeSpan Interval { get; }
        public bool SingleShot { get; }
        public bool StopOnShot { get; }
        public bool StopOnException { get; }

        public bool IsStarted { get; private set; }
        #endregion

        #region ctors & Dispose
        private LpTimer(
            TimeSpan interval,
            bool singleShot,
            Action<DateTime, CancellationToken> callback,
            Func<DateTime, CancellationToken, Task> asyncCallback,
            bool stopOnShot,
            bool stopOnException,
            string name,
            ILogger logger)
        {
            if (callback == null && asyncCallback == null)
            {
                throw new ArgumentException(
                    "Callback or AsyncCallback must be specified");
            }

            if ((string.IsNullOrEmpty(name)) ^ (logger == null))
            {
                throw new ArgumentException(
                    "Name and logger must be both specified or not specified");
            }

            Name = name;
            Interval = interval;
            SingleShot = singleShot;
            this.callback = callback;
            this.asyncCallback = asyncCallback;
            StopOnShot = stopOnShot;
            StopOnException = stopOnException;

            backend = new(Interval)
            {
                AutoReset = !SingleShot
            };
            backend.Elapsed += Backend_Elapsed;
            this.logger = logger;
        }

        public LpTimer(
            TimeSpan interval,
            bool singleShot,
            Action<DateTime, CancellationToken> callback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null)
            : this(
                  interval,
                  singleShot,
                  callback,
                  null,
                  stopOnShot,
                  stopOnException,
                  name,
                  logger)
        { }

        public LpTimer(
            TimeSpan interval,
            bool singleShot,
            Func<DateTime, CancellationToken, Task> asyncCallback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null)
            : this(
                  interval,
                  singleShot,
                  null,
                  asyncCallback,
                  stopOnShot,
                  stopOnException,
                  name,
                  logger)
        { }

        public void Dispose()
        {
            Stop();
            backend.Elapsed -= Backend_Elapsed;
            backend.Dispose();
        }
        #endregion

        #region Public interface
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            stopCts = new CancellationTokenSource();
            backend.Start();
            IsStarted = true;
            logger?.LogDebug("Timer {Name} started", Name);
        }

        public void Stop()
        {
            if (!IsStarted)
            {
                return;
            }

            backend.Stop();
            stopCts.Cancel();
            stopCts.Dispose();
            IsStarted = false;
            logger?.LogDebug("Timer {Name} stopped", Name);
        }

        public Task<int> ForceTickAsync(CancellationToken cancellationToken = default) =>
            Tick(DateTime.Now, cancellationToken);

        public int ForceTick(CancellationToken cancellationToken = default)
        {
            return Tick(DateTime.Now, cancellationToken)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        #endregion

        #region Internals 
        private async void Backend_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (StopOnShot)
            {
                backend.Stop();
            }

            if (SingleShot)
            {
                IsStarted = false;
            }

            var localExitCode = await Tick(e.SignalTime, stopCts.Token);
            if (localExitCode != ExitCodes.Success && StopOnException)
            {
                Stop();
                return;
            }

            if (IsStarted && !SingleShot && StopOnShot)
            {
                backend.Start();
            }
        }

        /// <summary>
        /// Returns local exit code/>
        /// </summary>
        private async Task<int> Tick(DateTime signalTime, CancellationToken cancellationToken)
        {
            var tickLogScope = logger?.BeginScope(
                "Timer {Name} tick at {signalTime:o}",
                Name,
                signalTime);
            var localExitCode = ExitCodes.Success;
            try
            {
                if (asyncCallback != null)
                {
                    await asyncCallback(signalTime, cancellationToken);
                }
                else
                {
                    callback(signalTime, cancellationToken);
                }

                return localExitCode;
            }
            catch (OperationCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // NOTE: do nothing, swallow
                    localExitCode = ExitCodes.Stopped;
                    logger?.LogDebug("Stopped");
                    return localExitCode;
                }

                localExitCode = ExitCodes.Unknown;
                logger?.LogError(ex, "Exception");
                return localExitCode;
            }
            catch (LpExitCodeException ex)
            {
                logger?.LogDebug(ex, "Local exit code received");
                localExitCode = ex.ExitCode;
                return localExitCode;
            }
            catch (Exception ex)
            {
                localExitCode = ExitCodes.Unknown;
                logger?.LogError(ex, "Exception");
                return localExitCode;
            }
            finally
            {
                LogRuntime(signalTime, localExitCode);
                tickLogScope?.Dispose();
            }
        }

        private void LogRuntime(DateTime startTime, int localExitCode)
        {
            if (logger == null)
            {
                return;
            }

            var endTime = DateTime.Now;
            var runtime = endTime - startTime;
            logger?.LogDebug(
                "Finised at {endTime:o}; LocalExitCode = {localExitCode}; Runtime = {runtime:c};",
                endTime,
                localExitCode.ExitCodeToString("c"),
                runtime);
        }
        #endregion
    }
}
