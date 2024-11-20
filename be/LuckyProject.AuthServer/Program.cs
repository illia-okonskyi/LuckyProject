using System.Threading.Tasks;

namespace LuckyProject.AuthServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await using var entryPoint = new EntryPoint(args);
            await entryPoint.ConfigureAsync();
            await entryPoint.RunAsync();
        }
    }
}
