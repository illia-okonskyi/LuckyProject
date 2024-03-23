using LuckyProject.Lib.Hosting.EntryPoint;
using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp
{
    public interface IEntryPoint : ILlGenericHostEntryPoint
    {
        Task RunAsync();
    }
}
