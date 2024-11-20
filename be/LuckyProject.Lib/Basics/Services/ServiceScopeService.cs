using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class ServiceScopeService : IServiceScopeService
    {
        #region Internals
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger logger;
        private readonly Dictionary<IServiceScope, string> namedScopes = new();
        private readonly Dictionary<IAsyncDisposable, string> namedAsyncScopes = new();
        #endregion

        #region Public properties
        public bool IsLoggingEnabled { get; set; }
        #endregion

        #region ctor
        public ServiceScopeService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<ServiceScopeService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }
        #endregion

        #region Public interface
        public IServiceScope CreateScope(
            string name = null,
            IServiceProvider serviceProvider = null)
        {
            var scope = serviceProvider != null
                ? serviceProvider.CreateScope()
                : serviceScopeFactory.CreateScope();
            if (string.IsNullOrEmpty(name))
            {
                return scope;
            }

            namedScopes.Add(scope, name);
            if (IsLoggingEnabled)
            {
                logger.LogDebug("Created Named ServiceScope: {name}", name);
            }
            return scope;
        }

        public void DisposeScope(IServiceScope scope)
        {
            if (scope == null)
            {
                return;
            }

            scope.Dispose();
            if (!namedScopes.ContainsKey(scope))
            {
                return;
            }

            namedScopes.Remove(scope, out var name);
            if (IsLoggingEnabled)
            {
                logger.LogDebug("Disposed Named ServiceScope: {name}", name);
            }
        }

        public AsyncServiceScope CreateAsyncScope(
            string name = null,
            IServiceProvider serviceProvider = null)
        {
            var scope = serviceProvider != null
                ? serviceProvider.CreateAsyncScope()
                : serviceScopeFactory.CreateAsyncScope();
            if (string.IsNullOrEmpty(name))
            {
                return scope;
            }

            namedAsyncScopes.Add(scope, name);
            if (IsLoggingEnabled)
            {
                logger.LogDebug("Created Named AsyncServiceScope: {name}", name);
            }

            return scope;
        }

        public async Task DisposeAsyncServiceScopeAsync(IAsyncDisposable scope)
        {
            if (scope == null)
            {
                return;
            }

            await scope.DisposeAsync();
            if (!namedAsyncScopes.ContainsKey(scope))
            {
                return;
            }

            namedAsyncScopes.Remove(scope, out var name);
            if (IsLoggingEnabled)
            {
                logger.LogDebug("Disposed Named AsyncServiceScope: {name}", name);
            }
        }
        #endregion
    }
}
