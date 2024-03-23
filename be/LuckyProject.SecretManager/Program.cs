using System.Threading.Tasks;

namespace LuckyProject.SecretManager
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
