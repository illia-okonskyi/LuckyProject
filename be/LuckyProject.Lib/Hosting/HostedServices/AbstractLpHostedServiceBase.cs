using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Hosting.HostedServices
{
    public abstract class AbstractLpHostedServiceBase : IHostedService, IDisposable
    {
        #region Constants
        public const string MainServiceName = "MainService";
        public const string MainServiceScopeName = "MainServiceScope";
        #endregion

        #region Internals
        private readonly IServiceScope serviceScope;
        #endregion

        #region Public/Protected properties
        protected string Name { get; }
        protected IServiceScopeService ServiceScopeService { get; }
        protected IHostApplicationLifetime AppLifetime { get; }
        protected IEnvironmentService EnvironmentService { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected ILogger Logger { get; }
        public bool IsPrimaryService { get; }
        public bool IsMainSingleService { get; }
        public int ExitCode { get; protected set; } = ExitCodes.Success;
        #endregion

        #region ctors
        /// <summary>
        /// Full constructor
        /// </summary>
        protected AbstractLpHostedServiceBase(
            IServiceScopeService serviceScopeService,
            string name,
            ILogger logger,
            bool isPrimaryService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            bool isMainSingleService)
        {
            if (serviceScopeService == null)
            {
                throw new ArgumentNullException(nameof(serviceScopeService));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (IsPrimaryService && appLifetime == null)
            {
                throw new ArgumentException(
                    "AppLifetime must be specified for Primary Service");
            }

            if (IsPrimaryService && environmentService == null)
            {
                throw new ArgumentException(
                    "EnvironmentService must be specified for Primary Service");
            }

            Name = name;
            ServiceScopeService = serviceScopeService;
            serviceScope = ServiceScopeService.CreateScope($"{Name}.{MainServiceScopeName}");
            AppLifetime = appLifetime;
            EnvironmentService = environmentService;

            ServiceProvider = serviceScope.ServiceProvider;
            Logger = logger;
            IsPrimaryService = isPrimaryService;
            IsMainSingleService = isMainSingleService;
        }

        /// <summary>
        /// Primary service
        /// </summary>
        protected AbstractLpHostedServiceBase(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            string name,
            ILogger logger = null)
            : this(serviceScopeService, name, logger, true, appLifetime, environmentService, false)
        { }

        /// <summary>
        /// Background service
        /// </summary>
        protected AbstractLpHostedServiceBase(
            IServiceScopeService serviceScopeService,
            string name,
            ILogger logger = null)
            : this(serviceScopeService, name, logger, false, null, null, false)
        { }

        /// <summary>
        /// Primary main single service
        /// </summary>
        protected AbstractLpHostedServiceBase(
            IServiceScopeService serviceScopeService,
            IHostApplicationLifetime appLifetime,
            IEnvironmentService environmentService,
            ILogger logger = null)
            : this(
                  serviceScopeService,
                  MainServiceName,
                  logger,
                  true,
                  appLifetime,
                  environmentService,
                  true)
        { }
        #endregion

        #region Dispose
        public void Dispose()
        {
            ServiceScopeService.DisposeScope(serviceScope);
        }
        #endregion

        #region IHostedService implementation
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var startLogScope = Logger?.BeginScope("Starting HostedService {Name}", Name);
            Logger?.LogDebug("Starting at {Now:o}...", DateTime.Now);
            try
            {
                await StartServiceBaseAsync(cancellationToken);
                Logger?.LogDebug("Started at {Now:o}", DateTime.Now);
            }
            catch (OperationCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    ExitCode = (await OnStartServiceCancelledBaseAsync()) ?? ExitCodes.Cancelled;
                    Logger?.LogDebug("Cancelled at {Now:o}...", DateTime.Now);
                    return;
                }

                Logger?.LogError(ex, "Exception at {Now:o}...", DateTime.Now);
                ExitCode = (await OnStartServiceExceptionBaseAsync(ex)) ?? ExitCodes.Unknown;
            }
            catch (LpExitCodeException ex)
            {
                Logger?.LogDebug(ex, "Exit code received");
                ExitCode = ex.ExitCode;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Exception at {Now:o}...", DateTime.Now);
                ExitCode = (await OnStartServiceExceptionBaseAsync(ex)) ?? ExitCodes.Unknown;
            }
            finally
            {
                startLogScope?.Dispose();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var stopLogScope = Logger?.BeginScope("Stopping HostedService {Name}", Name);
            try
            {
                await StopServiceBaseAsync(cancellationToken);
                Logger?.LogDebug("Stopped at {Now:o}", DateTime.Now);
            }
            catch (OperationCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    ExitCode = (await OnStopServiceCancelledBaseAsync()) ?? ExitCodes.Unknown;
                    Logger?.LogDebug("Cancelled at {Now:o}...", DateTime.Now);
                    return;
                }

                Logger?.LogError(ex, "Exception at {Now:o}...", DateTime.Now);
                ExitCode = (await OnStopServiceExceptionBaseAsync(ex)) ?? ExitCodes.Unknown;;
            }
            catch (LpExitCodeException ex)
            {
                Logger?.LogDebug(ex, "Exit code received");
                ExitCode = ex.ExitCode;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Exception at {Now:o}...", DateTime.Now);
                ExitCode = (await OnStopServiceExceptionBaseAsync(ex)) ?? ExitCodes.Unknown; ;
            }
            finally
            {
                stopLogScope?.Dispose();
                var exitCodeOverride = await OnBeforeServiceExitBaseAsync(ExitCode);
                if (exitCodeOverride.HasValue)
                {
                    ExitCode = exitCodeOverride.Value;
                }

                if (IsMainSingleService)
                {
                    Logger?.LogDebug(
                        "Setting application ExitCode = {ExitCode}",
                        ExitCode.ExitCodeToString("c"));
                    EnvironmentService.ExitCode = ExitCode;
                }
                ServiceScopeService.DisposeScope(serviceScope);
            }
        }
        #endregion

        #region Protected & virtual interface
        protected void RequestStopApplication(int? exitCode = null)
        {
            if (!IsPrimaryService)
            {
                throw new InvalidOperationException("Not a primary service");
            }

            if (exitCode.HasValue)
            {
                ExitCode = exitCode.Value;
            }

            Logger?.LogDebug(
                "Requested application stop. ExitCode = {ExitCode}",
                ExitCode.ExitCodeToString("c"));
            EnvironmentService.ExitCode = ExitCode;
            AppLifetime.StopApplication();
        }

        protected virtual Task StartServiceBaseAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        /// <summary>
        /// Must return exit code or null (default will be used)
        /// </summary>
        protected virtual Task<int?> OnStartServiceCancelledBaseAsync() =>
            Task.FromResult<int?>(null);
        /// <summary>
        /// Must return exit code or null (default will be used)
        /// </summary>
        protected virtual Task<int?> OnStartServiceExceptionBaseAsync(Exception ex) =>
            Task.FromResult<int?>(null);

        protected virtual Task StopServiceBaseAsync(CancellationToken cancellationToken) =>
            Task.CompletedTask;
        /// <summary>
        /// Must return exit code or null (default will be used)
        /// </summary>
        protected virtual Task<int?> OnStopServiceCancelledBaseAsync() =>
            Task.FromResult<int?>(null);
        /// <summary>
        /// Must return exit code or null (default will be used)
        /// </summary>
        protected virtual Task<int?> OnStopServiceExceptionBaseAsync(Exception ex) =>
            Task.FromResult<int?>(null);

        /// <summary>
        /// Must return exit code or null (default will be used)
        /// </summary>
        protected virtual Task<int?> OnBeforeServiceExitBaseAsync(int exitCode) =>
            Task.FromResult<int?>(null);
        #endregion
    }
}
