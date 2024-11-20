using LuckyProject.Lib.Hosting.EntryPoint;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer
{
    internal interface IEntryPoint : IWebHostEntryPoint
    {
        Task RunAsync();
    }
}
