using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    public interface IWebHostEntryPoint : IAsyncDisposable
    {
        WebApplication App { get; }
        Task ConfigureAsync();
    }
}
