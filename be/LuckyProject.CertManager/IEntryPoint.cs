using LuckyProject.Lib.Hosting.EntryPoint;
using System.Threading.Tasks;

namespace LuckyProject.CertManager
{
    public interface IEntryPoint : IGenericHostEntryPoint
    {
        Task RunAsync();
    }
}
