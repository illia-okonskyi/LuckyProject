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
        public IHost Host { get; protected set; }
        #endregion

        #region Configuration builder forwarding
        protected HostApplicationBuilder HostBuilder { get; set; }
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

        public abstract Task ConfigureAsync();

        protected void BuildHost()
        {
            Host = HostBuilder.Build();
            HostBuilder = null;
        }
    }
}
