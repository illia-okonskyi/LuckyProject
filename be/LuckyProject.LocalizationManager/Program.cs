using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Lib.Init.InitAssembly.Init();
            using var entryPoint = new EntryPoint<App>(args);
            await entryPoint.ConfigureAsync();
            await entryPoint.RunAsync();
        }
    }
}
