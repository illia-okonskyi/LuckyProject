using LuckyProject.Lib.Hosting.EntryPoint;

namespace LuckyProject.AuthServer
{
    public interface IEntryPoint : IWebHostEntryPoint
    {
        Task RunAsync();
    }
}
