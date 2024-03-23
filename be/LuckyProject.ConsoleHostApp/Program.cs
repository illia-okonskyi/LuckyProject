using System.Threading.Tasks;

namespace LuckyProject.ConsoleHostApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var entryPoint = new EntryPoint(args);
            await entryPoint.ConfigureAsync();
            await entryPoint.RunAsync();
        }
    }
}
