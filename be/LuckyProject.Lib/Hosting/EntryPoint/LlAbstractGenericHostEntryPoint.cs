using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    /// <summary>
    /// For documentation reference see the
    /// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
    /// </summary>
    public abstract class LlAbstractGenericHostEntryPoint : ILlGenericHostEntryPoint
    {
        #region Public interface
        public IHost Host { get; private set; }
        #endregion

        #region Configuration builder forwarding
        protected HostApplicationBuilder HostBuilder { get; private set; }
        #endregion

        #region ctor & Dispose
        protected LlAbstractGenericHostEntryPoint(string[] args)
        {
            HostBuilder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
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
            await ConfigureHostedServicesAsync();
            await ConfigureFinishAsync();
            Host = HostBuilder.Build();
            HostBuilder = null;
        }

        protected virtual Task ConfigureInitAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureConfigurationAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureLoggingAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureServicesAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureHostedServicesAsync() { return Task.CompletedTask; }
        protected virtual Task ConfigureFinishAsync() { return Task.CompletedTask; }
        #endregion
    }
}
