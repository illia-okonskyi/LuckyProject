using LuckyProject.Lib.Hosting.EntryPoint;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager
{
    public interface IEntryPoint<TApplication> : IGenericHostEntryPoint
        where TApplication : Application, new()
    {
        Task RunAsync();
    }
}
