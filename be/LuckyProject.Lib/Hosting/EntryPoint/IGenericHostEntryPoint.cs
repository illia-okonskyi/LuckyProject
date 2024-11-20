using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    public interface IGenericHostEntryPoint : IDisposable
    {
        string[] Args { get; }
        IHost Host { get; }
        Task ConfigureAsync(string[] args = null);
    }
}
