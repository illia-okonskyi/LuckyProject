using Microsoft.Extensions.Hosting;

namespace LuckyProject.Lib.Hosting.HostedServices
{
    public interface ILpHostedService : IHostedService
    {
        bool IsPrimaryService { get; }
        bool IsMainSingleService { get; }
        int ExitCode { get; }
    }
}
