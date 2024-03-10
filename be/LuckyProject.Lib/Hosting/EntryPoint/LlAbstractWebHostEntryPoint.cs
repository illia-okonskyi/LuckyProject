using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    /// <summary>
    /// For documentation reference see the
    /// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-8.0
    /// </summary>
    public abstract class LlAbstractWebHostEntryPoint : ILlWebHostEntryPoint
    {
        #region Public interface
        public IWebHost Host { get; private set; }
        #endregion

        #region Configuration builder forwarding
        protected IWebHostBuilder HostBuilder { get; private set; }
        #endregion

        #region ctor & Dispose
        protected LlAbstractWebHostEntryPoint(string[] args)
        {
            HostBuilder = WebHost.CreateDefaultBuilder(args);
        }

        public void Dispose()
        {
            if (Host != null)
            {
                Host.Dispose();
                Host = null;
            }
        }
        #endregion

        #region ConfigureAsync
        public async Task ConfigureAsync()
        {
            await ConfigureInitAsync();
            await ConfigureConfigurationAsync();
            await ConfigureLoggingAsync();
            await ConfigureServicesAsync();
            await ConfigureWebServerAsync();
            await ConfigureHostedServicesAsync();
            Host = HostBuilder.Build();
            HostBuilder = null;
            await ConfigureHostAppAsync();
            await ConfigureFinishAsync();
        }

        protected virtual Task ConfigureInitAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureConfigurationAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureLoggingAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureServicesAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureWebServerAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureHostedServicesAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureHostAppAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureFinishAsync() { return Task.CompletedTask; }
        #endregion
    }
}
