using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    /// <summary>
    /// For documentation reference see the
    /// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
    /// </summary>
    public abstract class AbstractGenericHostEntryPoint : IGenericHostEntryPoint
    {
        #region Public interface
        public string[] Args { get; private set; }
        public IHost Host { get; protected set; }
        #endregion

        #region Configuration builder forwarding
        protected HostApplicationBuilder HostBuilder { get; set; }

        #endregion

        #region ctor & Dispose
        protected AbstractGenericHostEntryPoint(string[] args = null)
        {
            Args = args;
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

        public async Task ConfigureAsync(string[] args = null)
        {
            Args = args;
            await ConfigureAsyncImpl();
        }

        protected abstract Task ConfigureAsyncImpl();

        protected void BuildHost()
        {
            Host = HostBuilder.Build();
            HostBuilder = null;
        }

    }
}
