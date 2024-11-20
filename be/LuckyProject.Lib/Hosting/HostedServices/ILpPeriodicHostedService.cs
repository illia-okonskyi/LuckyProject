using System;

namespace LuckyProject.Lib.Hosting.HostedServices
{
    public interface ILpPeriodicHostedService : ILpHostedService
    {
        TimeSpan Interval { get; }
        bool IsExecuting { get; }
    }

}
