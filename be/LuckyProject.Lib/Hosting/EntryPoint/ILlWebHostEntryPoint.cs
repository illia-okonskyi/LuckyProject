using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    public interface ILlWebHostEntryPoint : IDisposable
    {
        IWebHost Host { get; }
        Task ConfigureAsync();
    }
}
