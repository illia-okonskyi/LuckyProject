using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    public interface ILlGenericHostEntryPoint : IDisposable
    {
        IHost Host { get; }
        Task ConfigureAsync();
    }
}
