using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Imports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Processes
{
    public class LpConsoleProcess : ILpConsoleProcess
    {
        #region Internals
        private readonly IRuntimeInformationService rtiService;
        private readonly Process backend;
        #endregion

        #region Public properties
        public LpProcessStartParams StartParams { get; }
        public int ExitCode => backend.ExitCode;
        public bool IsStarted { get; private set; }
        public bool IsFinished => IsStarted && backend.HasExited;
        public bool IsRunning => IsStarted && !IsFinished;
        #endregion

        #region ctor & Dispose
        public LpConsoleProcess(
            IRuntimeInformationService rtiService,
            LpProcessStartParams startParams)
        {
            this.rtiService = rtiService;
            StartParams = startParams;

            var psi = new ProcessStartInfo(startParams.FileName, startParams.Arguments)
            {
                RedirectStandardInput = true,
                StandardInputEncoding = startParams.Encoding,
                RedirectStandardOutput = true,
                StandardOutputEncoding = startParams.Encoding,
                RedirectStandardError = true,
                StandardErrorEncoding = startParams.Encoding,
                UseShellExecute = false,
                WorkingDirectory = startParams.WorkingDir
            };
            foreach (var kvp in startParams.Environment)
            {
                psi.EnvironmentVariables.Add(kvp.Key, kvp.Value);
            }

            backend = new Process { StartInfo = psi };
            backend.Exited += Backend_Exited;
        }

        public void Dispose()
        {
            backend.Exited -= Backend_Exited;
            backend.Dispose();
        }
        private void Backend_Exited(object sender, EventArgs e)
        {
            IsStarted = false;
        }
        #endregion

        #region Public interface
        #region Start / Kill / Join
        public void Start()
        {
            if (IsStarted)
            {
                return;
            }

            if (!backend.Start())
            {
                throw new InvalidOperationException("Failed to start process");
            }

            IsStarted = true;
        }

        public void Kill(bool entireProcessTree = false) => backend.Kill(entireProcessTree);

        public async Task<int> JoinAsync(CancellationToken cancellationToken = default)
        {
            if (IsFinished)
            {
                return ExitCode;
            }

            if (!IsStarted)
            {
                throw new InvalidOperationException("Process not started");
            }

            await backend.WaitForExitAsync(cancellationToken);
            return ExitCode;
        }
        #endregion

        #region Signals
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public async Task<int> SendSigkillAndJoinAsync()
        {
            var osPlatform = rtiService.OsPlatform;
            if (osPlatform == LpOsPlatform.Linux)
            {
                var errorCode = LpImports.Linux.kill(backend.Id, (int)LpSignum.SIGKILL);
                if (errorCode != 0)
                {
                    throw new InvalidOperationException($"kill failed: {errorCode}");
                }
                return await JoinAsync();
            }

            if (osPlatform != LpOsPlatform.Windows)
            {
                throw new PlatformNotSupportedException("Windows or Linux supported only");
            }


            var consoleAttached = LpImports.Windows.AttachConsole((uint)backend.Id);
            var handlerSet = false;
            try
            {
                handlerSet = LpImports.Windows.SetConsoleCtrlHandler(null, true);
                if (handlerSet)
                {
                    if (!LpImports.Windows.GenerateConsoleCtrlEvent(
                        LpImports.Windows.CTRL_C_EVENT,
                        0))
                    {
                        throw new InvalidOperationException("GenerateConsoleCtrlEvent failed");
                    }
                }
                return await JoinAsync();
            }
            finally
            {
                if (handlerSet)
                {
                    LpImports.Windows.SetConsoleCtrlHandler(null, false);
                }

                if (consoleAttached)
                {
                    LpImports.Windows.FreeConsole();
                }
            }
        }

        [SupportedOSPlatform("linux")]
        public void PostSignal(LpSignum signal)
        {
            var osPlatform = rtiService.OsPlatform;
            if (osPlatform != LpOsPlatform.Linux)
            {
                throw new PlatformNotSupportedException("Only Linux supported");
            }

            var errorCode = LpImports.Linux.kill(backend.Id, (int)signal);
            if (errorCode != 0)
            {
                throw new InvalidOperationException($"kill failed: {errorCode}");
            }
        }
        #endregion

        #region ExecAsync
        [SupportedOSPlatform("windows")]
        [SupportedOSPlatform("linux")]
        public async Task<int> ExecAsync(
            LpConsoleProcessExecParams execParams,
            CancellationToken cancellationToken = default)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Process is running");
            }

            using var ctx = new ExecAsyncContext(
                backend,
                JoinAsync,
                SendSigkillAndJoinAsync,
                execParams,
                cancellationToken);
            Start();
            while (true)
            {
                var tasks = ctx.Update();
                Task completedTask = null;
                try
                {
                    completedTask = await Task.WhenAny(tasks);
                }
                catch (OperationCanceledException)
                {
                    // NOTE: Do nothing, forward
                    throw;
                }
                catch (Exception)
                {
                    return ExitCodes.Unknown;
                }

                if (completedTask == ctx.DelayTask)
                {
                    ctx.DelayTask = null;
                    continue;
                }

                if (completedTask == ctx.StdInTask)
                {
                    ctx.StdInTask = null;
                    continue;
                }

                if (completedTask == ctx.StdOutTask)
                {
                    ctx.StdOutTask = null;
                    continue;
                }

                if (completedTask == ctx.StdErrTask)
                {
                    ctx.StdOutTask = null;
                    continue;
                }

                if (completedTask == ctx.JoinTask || completedTask == ctx.SigKillTask)
                {
                    //ctx.InnerCts.Cancel();
                    return ExitCode;
                }

                throw new InvalidOperationException("Unreachable code");
            }
        }

        private class ExecAsyncContext : IDisposable
        {
            private const int TempBufferSize = 128;
            private readonly Process backend;
            private readonly Func<CancellationToken, Task<int>> joinAsync;
            private readonly Func<Task> sendSigkillAndJoinAsync;
            private readonly LpConsoleProcessExecParams execParams;


            public Task JoinTask { get; set; }
            public Task DelayTask { get; set; }
            public Task SigKillTask { get; set; }
            public Task StdInTask { get; set; }
            public Task StdOutTask { get; set; }
            public Task StdErrTask { get; set; }
            public bool MoreStdIn { get; set; }
            public bool MoreStdOut { get; set; }
            public bool MoreStdErr { get; set; }
            public char[] StdOutBuffer { get; set; }
            public char[] StdErrBuffer { get; set; }
            public LpStdStreamReadWriteMode StdOutReadMode { get; set; }
            public LpStdStreamReadWriteMode StdErrReadMode { get; set; }
            public CancellationTokenSource InnerCts { get; } = new();
            public CancellationTokenSource ExecCts { get; }

            public ExecAsyncContext(
                Process backend,
                Func<CancellationToken, Task<int>> joinAsync,
                Func<Task> sendSigkillAndJoinAsync,
                LpConsoleProcessExecParams execParams,
                CancellationToken cancellationToken)
            {
                this.backend = backend;
                this.joinAsync = joinAsync;
                this.sendSigkillAndJoinAsync = sendSigkillAndJoinAsync;
                this.execParams = execParams;

                MoreStdIn = execParams.StdInCallback != null;
                MoreStdOut = execParams.StdOutCallback != null;
                MoreStdErr = execParams.StdErrCallback != null;
                if (MoreStdOut)
                {
                    StdOutBuffer = new char[TempBufferSize];
                }
                if (MoreStdErr)
                {
                    StdErrBuffer = new char[TempBufferSize];
                }
                StdOutReadMode = execParams.StdOutDefaultReadMode;
                StdErrReadMode = execParams.StdErrDefaultReadMode;
                ExecCts = CancellationTokenSource.CreateLinkedTokenSource(
                    InnerCts.Token,
                    cancellationToken);
            }

            public void Dispose()
            {
                ExecCts.Dispose();
                InnerCts.Dispose();
            }

            public List<Task> Update()
            {
                var ct = ExecCts.Token;
                JoinTask ??= joinAsync(ct);
                DelayTask ??= Task.Delay(TimeoutDefaults.Shorter, ct);

                if (execParams.SigKillCallback != null && SigKillTask == null)
                {
                    SigKillTask = Task.Run(async () =>
                    {
                        await execParams.SigKillCallback(ct);
                        await sendSigkillAndJoinAsync();
                    },
                    ct);
                }

                if (MoreStdIn && StdInTask == null)
                {
                    StdInTask = Task.Run(async () =>
                    {
                        try
                        {
                            var p = await execParams.StdInCallback(ct);
                            if (p.Mode == LpStdStreamReadWriteMode.End)
                            {
                                MoreStdIn = false;
                                return;
                            }

                            if (p.Mode == LpStdStreamReadWriteMode.Chunk)
                            {
                                await backend.StandardInput.WriteAsync(p.Value.AsMemory(), ct);
                            }
                            else
                            {
                                await backend.StandardInput.WriteLineAsync(p.Value.AsMemory(), ct);
                            }
                        }
                        catch
                        {
                            MoreStdIn = false;
                        }
                    },
                    ct);
                }

                if (MoreStdOut && StdOutTask == null)
                {
                    StdOutTask = Task.Run(async () =>
                    {
                        string s = null;
                        try
                        {
                            if (StdOutReadMode == LpStdStreamReadWriteMode.Chunk)
                            {
                                var charsRead = await backend.StandardOutput.ReadAsync(
                                    StdOutBuffer,
                                    ct);
                                if (charsRead > 0)
                                {
                                    s = new string(StdOutBuffer[0..charsRead]);
                                }
                            }
                            else
                            {
                                s = await backend.StandardOutput.ReadLineAsync(ct);
                            }

                            if (s == null)
                            {
                                MoreStdOut = false;
                                return;
                            }

                            var p = await execParams.StdOutCallback(
                                new() { Value = s, Mode = StdOutReadMode },
                                ct);
                            StdOutReadMode = p.Mode;
                            if (p.Mode == LpStdStreamReadWriteMode.End)
                            {
                                MoreStdOut = false;
                            }
                        }
                        catch
                        {
                            MoreStdOut = false;
                        }
                    },
                    ct);
                }

                if (MoreStdErr && StdErrTask == null)
                {
                    StdErrTask = Task.Run(async () =>
                    {
                        string s = null;
                        try
                        {
                            if (StdErrReadMode == LpStdStreamReadWriteMode.Chunk)
                            {
                                var charsRead = await backend.StandardError.ReadAsync(
                                    StdErrBuffer,
                                    ct);
                                if (charsRead > 0)
                                {
                                    s = new string(StdErrBuffer[0..charsRead]);
                                }
                            }
                            else
                            {
                                s = await backend.StandardError.ReadLineAsync(ct);
                            }

                            if (s == null)
                            {
                                MoreStdErr = false;
                                return;
                            }

                            var p = await execParams.StdErrCallback(
                                new() { Value = s, Mode = StdErrReadMode },
                                ct);
                            StdErrReadMode = p.Mode;
                            if (p.Mode == LpStdStreamReadWriteMode.End)
                            {
                                MoreStdErr = false;
                            }
                        }
                        catch
                        {
                            MoreStdErr = false;
                        }
                    },
                    ct);
                }

                var tasks = new List<Task> { JoinTask, DelayTask };
                if (SigKillTask != null)
                {
                    tasks.Add(SigKillTask);
                }
                if (StdInTask != null)
                {
                    tasks.Add(StdInTask);
                }
                if (StdOutTask != null)
                {
                    tasks.Add(StdOutTask);
                }
                if (StdErrTask != null)
                {
                    tasks.Add(StdErrTask);
                }

                return tasks;
            }
        }
        #endregion

        #region Stats
        public LpProcessStats GetCurrentStats()
        {
            if (!IsRunning)
            {
                return new LpProcessStats
                {
                    IsStarted = IsStarted,
                    IsFinished = IsFinished,
                    StartTime = IsStarted ? backend.StartTime : null,
                    ExitTime = IsFinished ? backend.ExitTime : null,
                    ExitCode = IsFinished ? ExitCode : null
                };
            }

            backend.Refresh();
            return new LpProcessStats
            {
                IsStarted = IsStarted,
                IsFinished = IsFinished,
                StartTime = backend.StartTime,
                BasePriority = backend.BasePriority,
                HandleCount = backend.HandleCount,
                Id = backend.Id,
                MainModule = backend.MainModule,
                MaxWorkingSet = backend.MaxWorkingSet,
                MinWorkingSet = backend.MinWorkingSet,
                Modules = backend.Modules.Cast<ProcessModule>().ToList(),
                NonpagedSystemMemorySize64 = backend.NonpagedSystemMemorySize64,
                PagedMemorySize64 = backend.PagedMemorySize64,
                PagedSystemMemorySize64 = backend.PagedSystemMemorySize64,
                PeakPagedMemorySize64 = backend.PeakPagedMemorySize64,
                PeakVirtualMemorySize64 = backend.PeakVirtualMemorySize64,
                PeakWorkingSet64 = backend.PeakWorkingSet64,
                PriorityBoostEnabled = backend.PriorityBoostEnabled,
                PriorityClass = backend.PriorityClass,
                PrivateMemorySize64 = backend.PrivateMemorySize64,
                PrivilegedProcessorTime = backend.PrivilegedProcessorTime,
                Responding = backend.Responding,
                Threads = backend.Threads.Cast<ProcessThread>().ToList(),
                TotalProcessorTime = backend.TotalProcessorTime,
                UserProcessorTime = backend.UserProcessorTime,
                VirtualMemorySize64 = backend.VirtualMemorySize64,
                WorkingSet64 = backend.WorkingSet64
            };
        }
        #endregion
        #endregion
    }
}
