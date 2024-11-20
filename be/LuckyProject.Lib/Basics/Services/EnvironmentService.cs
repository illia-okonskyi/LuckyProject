using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LuckyProject.Lib.Basics.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        #region Internals & ctor
        private readonly IHostEnvironment hostEnvironment;

        public EnvironmentService(IHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }
        #endregion

        #region IHostEnvironment
        public string ApplicationName
        {
            get => hostEnvironment.ApplicationName;
            set => hostEnvironment.ApplicationName = value;
        }

        public IFileProvider ContentRootFileProvider
        {
            get => hostEnvironment.ContentRootFileProvider;
            set => hostEnvironment.ContentRootFileProvider = value;
        }

        public string ContentRootPath
        {
            get => hostEnvironment.ContentRootPath;
            set => hostEnvironment.ContentRootPath = value;
        }

        public string EnvironmentName
        {
            get => hostEnvironment.EnvironmentName;
            set => hostEnvironment.EnvironmentName = value;
        }
        #endregion

        #region Environment static methods
        public int ProcessorCount => Environment.ProcessorCount;
        public bool IsPrivilegedProcess => Environment.IsPrivilegedProcess;

        public string GetEnvironmentVariable(string variable) =>
            Environment.GetEnvironmentVariable(variable);
        public string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target) =>
            Environment.GetEnvironmentVariable(variable, target);
        public Dictionary<string, string> GetEnvironmentVariables(
            EnvironmentVariableTarget target)
        {
            var result = new Dictionary<string, string>();
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                result.Add((string)de.Key, (string)de.Value);
            }
            return result;
        }

        public void SetEnvironmentVariable(string variable, string value) =>
            Environment.SetEnvironmentVariable(variable, value);
        public void SetEnvironmentVariable(
            string variable,
            string value,
            EnvironmentVariableTarget target) =>
            Environment.SetEnvironmentVariable(variable, value, target);
        public string[] GetCommandLineArgs() => Environment.GetCommandLineArgs();

        public string CommandLine => Environment.CommandLine;

        public string CurrentDirectory
        {
            get => Environment.CurrentDirectory;
            set => Environment.CurrentDirectory = value;
        }

        public string ExpandEnvironmentVariables(string name) =>
            Environment.ExpandEnvironmentVariables(name);
        public string GetFolderPath(Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);
        public string GetFolderPath(
            Environment.SpecialFolder folder,
            Environment.SpecialFolderOption option) =>
            Environment.GetFolderPath(folder, option);
        public int ProcessId => Environment.ProcessId;
        public string ProcessPath => Environment.ProcessPath;
        public bool Is64BitProcess => Environment.Is64BitProcess;
        public bool Is64BitOperatingSystem => Environment.Is64BitOperatingSystem;

        public string NewLine => Environment.NewLine;

        public OperatingSystem OsVersion => Environment.OSVersion;
        public Version Version => Environment.Version;

        public string StackTrace => Environment.StackTrace;
        public int SystemPageSize => Environment.SystemPageSize;
        public int CurrentManagedThreadId => Environment.CurrentManagedThreadId;

        [DoesNotReturn]
        public void Exit(int exitCode) => Environment.Exit(exitCode);

        public int ExitCode
        {
            get => Environment.ExitCode;
            set => Environment.ExitCode = value;
        }

        public void FailFast(string message) => Environment.FailFast(message);
        public void FailFast(string message, Exception exception) =>
            Environment.FailFast(message, exception);  

        public int TickCount => Environment.TickCount;
        public long TickCount64 => Environment.TickCount64;
        #endregion
    }
}
