using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Hosting.HostedServices;
using LuckyProject.Lib.WinUi.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public class WinUiHostedService<TApplication> : AbstractLpSingleRunLpHostedService
        where TApplication : Application, new()
    {
        #region Internals
        private readonly ILogger logger;
        #endregion

        #region ctor
        public WinUiHostedService( 
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILpTimerFactory timerFactory,
            ILogger<WinUiHostedService<TApplication>> logger)
            : base(
                  serviceScopeService,
                  appLifetime,
                  environmentService,
                  timerFactory,
                  "WinUiHostedService",
                  logger)
        {
            this.logger = logger;
            StaticRootServiceProviderContainer.RootServiceProvider = ServiceProvider;
            appLifetime.ApplicationStopped.Register(OnHostStopped);
        }
        #endregion

        #region Execute
        protected override Task ExecuteSingleRunServiceAsync(
            CancellationToken cancellationToken)
        {
            var appThread = new Thread(RunApplication);
            appThread.Name = "WinUi3 Main Thread";
            appThread.SetApartmentState(ApartmentState.STA);
            appThread.Start();
            appThread.Join();
            return Task.CompletedTask;
        }

        private void RunApplication()
        {
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Application app = null;
            int? localExitCode = null;
            Application.Start((p) =>
            {
                try
                {
                    var context = new DispatcherQueueSynchronizationContext(
                        DispatcherQueue.GetForCurrentThread());
                    SynchronizationContext.SetSynchronizationContext(context);
                    app = new TApplication();
                    app.UnhandledException += App_UnhandledException;
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Error");
                    localExitCode = ExitCodes.Unknown;
                }
            });

            app.UnhandledException -= App_UnhandledException;
            RequestStopApplication(localExitCode ?? app.GetLpApplicationRoot().ExitCode);
        }

        private void App_UnhandledException(
            object sender,
            Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            logger.LogCritical(e.Exception, "Unhandled exception");
            ((Application)sender).GetLpApplicationRoot().ExitApp(ExitCodes.Unknown);
        }

        private void OnHostStopped()
        {
            EnvironmentService.Exit(ExitCode);
        }
        #endregion
    }
}