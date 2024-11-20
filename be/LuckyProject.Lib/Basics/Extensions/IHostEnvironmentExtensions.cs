using Microsoft.Extensions.Hosting;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class IHostEnvironmentExtensions
    {
        public static bool IsLpDevelopment(this IHostEnvironment e) =>
            e.EnvironmentName.StartsWith("Development");
    }
}
