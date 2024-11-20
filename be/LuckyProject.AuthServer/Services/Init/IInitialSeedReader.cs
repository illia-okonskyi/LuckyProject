using System.Threading.Tasks;
using System;

namespace LuckyProject.AuthServer.Services.Init
{
    public interface IInitialSeedReader
    {
        InitialSeedOptions Options { get; }

        Task<DateTime?> GetSeedTimeUtcAsync();
    }
}
