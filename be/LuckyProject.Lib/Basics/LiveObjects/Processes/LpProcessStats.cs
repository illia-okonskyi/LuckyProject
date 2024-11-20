using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LuckyProject.Lib.Basics.LiveObjects.Processes
{
    public class LpProcessStats
    {
        public bool IsStarted { get; init; }
        public bool IsFinished { get; init; }
        public bool IsRunning => IsStarted && !IsFinished;
        public int? BasePriority { get; init; }
        public int? ExitCode { get; init; }
        public DateTime? ExitTime { get; init; }
        public int? HandleCount { get; init; }
        public int? Id { get; init; }
        public ProcessModule MainModule { get; init; }
        public IntPtr? MaxWorkingSet { get; init; }
        public IntPtr? MinWorkingSet { get; init; }
        public List<ProcessModule> Modules { get; init; }
        public long? NonpagedSystemMemorySize64 { get; init; }
        public long? PagedMemorySize64 { get; init; }
        public long? PagedSystemMemorySize64 { get; init; }
        public long? PeakPagedMemorySize64 { get; init; }
        public long? PeakVirtualMemorySize64 { get; init; }
        public long? PeakWorkingSet64 { get; init; }
        public bool? PriorityBoostEnabled { get; init; }
        public ProcessPriorityClass? PriorityClass { get; init; }
        public long? PrivateMemorySize64 { get; init; }
        public TimeSpan? PrivilegedProcessorTime { get; init; }
        public bool? Responding { get; init; }
        public DateTime? StartTime { get; init; }
        public List<ProcessThread> Threads { get; init; }
        public TimeSpan? TotalProcessorTime { get; init; }
        public TimeSpan? UserProcessorTime { get; init; }
        public long? VirtualMemorySize64 { get; init; }
        public long? WorkingSet64 { get; init; }
    }
}
