using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IEnvironmentService : IHostEnvironment
    {
        #region Environment static methods
        int ProcessorCount { get; }
        bool IsPrivilegedProcess { get; }

        string GetEnvironmentVariable(string variable);
        string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
        Dictionary<string, string> GetEnvironmentVariables(EnvironmentVariableTarget target);

        void SetEnvironmentVariable(string variable, string value);
        void SetEnvironmentVariable(
            string variable,
            string value,
            EnvironmentVariableTarget target);
        string[] GetCommandLineArgs();

        string CommandLine { get; }

        string CurrentDirectory { get; set; }

        string ExpandEnvironmentVariables(string name);
        string GetFolderPath(Environment.SpecialFolder folder);
        string GetFolderPath(
            Environment.SpecialFolder folder,
            Environment.SpecialFolderOption option);
        int ProcessId { get; }
        string ProcessPath { get; }
        bool Is64BitProcess { get; }
        bool Is64BitOperatingSystem { get; }

        string NewLine { get; }

        OperatingSystem OsVersion { get; }
        Version Version { get; }

        string StackTrace { get; }
        int SystemPageSize { get; }
        int CurrentManagedThreadId { get; }

        [DoesNotReturn]
        void Exit(int exitCode);

        int ExitCode { get; set; }

        void FailFast(string message);
        void FailFast(string message, Exception exception);

        int TickCount { get; }
        long TickCount64 { get; }
        #endregion
    }
}
