using System.Threading.Tasks;

namespace LuckyProject.CertManager
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
