using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    /// <summary>
    /// Service for enabling named <see cref="IServiceScope"/>
    /// </summary>
    public interface IServiceScopeService
    {
        bool IsLoggingEnabled { get; set; }

        IServiceScope CreateScope(string name = null, IServiceProvider serviceProvider = null);
        void DisposeScope(IServiceScope scope);
        AsyncServiceScope CreateAsyncScope(
            string name = null,
            IServiceProvider serviceProvider = null);
        Task DisposeAsyncServiceScopeAsync(IAsyncDisposable scope);
    }
}
