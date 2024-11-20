using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Processes
{
    public interface ILpConsoleProcess : IDisposable
    {
        #region Properties
        LpProcessStartParams StartParams { get; }
        int ExitCode { get; }
        bool IsStarted { get; }
        bool IsFinished { get; }
        bool IsRunning { get; }
        #endregion

        #region Start / Kill / Join
        void Start();
        void Kill(bool entireProcessTree = false);
        Task<int> JoinAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Signals
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        Task<int> SendSigkillAndJoinAsync();

        [SupportedOSPlatform("linux")]
        void PostSignal(LpSignum signal);
        #endregion

        #region ExecAsync
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        Task<int> ExecAsync(
            LpConsoleProcessExecParams execParams,
            CancellationToken cancellationToken = default);
        #endregion

        #region Stats
        public LpProcessStats GetCurrentStats();
        #endregion
    }
}
