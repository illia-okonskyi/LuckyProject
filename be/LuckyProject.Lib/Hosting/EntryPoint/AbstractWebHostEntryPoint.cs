using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.EntryPoint
{
    public abstract class AbstractWebHostEntryPoint : IWebHostEntryPoint
    {
        #region Public interface
        public WebApplication App { get; private set; }
        #endregion

        #region Configuration builder forwarding
        protected WebApplicationBuilder AppBuilder { get; set; }
        #endregion

        #region ctor & Dispose
        protected AbstractWebHostEntryPoint(string[] args)
        {
            AppBuilder = WebApplication.CreateBuilder(args);
        }

        public async ValueTask DisposeAsync()
        {
            if (App != null)
            {
                await App.DisposeAsync();
                App = null;
            }
        }
        #endregion
        public abstract Task ConfigureAsync();

        protected void BuildApp()
        {
            App = AppBuilder.Build();
            AppBuilder = null;
        }
    }
}
