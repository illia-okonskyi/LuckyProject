using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Models;
using LuckyProject.AuthServer.Services.Cors.Requests;
using LuckyProject.AuthServer.Services.Cors.Validators;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Cors
{
    public class AuthServerCorsService : IAuthServerCorsService
    {
        #region Internals & ctor
        private readonly WebServerOptions webServerOptions;
        private readonly IServiceScopeService scopeService;
        private readonly IValidationService validationService;
        private readonly IStringService stringService;

        public AuthServerCorsService(
            IOptions<WebServerOptions> webServerOptions,
            IServiceScopeService scopeService,
            IValidationService validationService,
            IStringService stringService)
        {
            this.webServerOptions = webServerOptions.Value;
            this.scopeService = scopeService;
            this.validationService = validationService;
            this.stringService = stringService;
        }
        #endregion

        #region Public interface
        public async Task CreateConfigAsync(
            CreateConfigRequest request,
            CancellationToken cancellationToken = default)
        {
            using var scope = scopeService.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthServerDbContext>();
            await CreateConfigAsync(dbContext, request, cancellationToken);
        }

        public async Task CreateConfigAsync(
            AuthServerDbContext dbContext,
            CreateConfigRequest request,
            CancellationToken cancellationToken = default)
        {
            validationService.EnsureValid(request, new CreateConfigRequestValidator());
            var configs = request.Origins
                .Select(o => new AuthServerCorsConfig
                {
                    ClientId = request.ClientId,
                    Origin = stringService.GetUriSchemeAndAuthority(o, false)
                }).ToList();
            dbContext.CorsConfig.AddRange(configs);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Uri>> GetClientOriginsAsync(
            AuthServerDbContext dbContext,
            string clientId,
            CancellationToken cancellationToken = default)
        {
            var origins = await dbContext.CorsConfig
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => c.Origin)
                .ToListAsync(cancellationToken);
            return origins.Select(o => new Uri(o)).ToList();
        }

        public async Task<PaginatedList<Uri>> GetClientOriginsAsync(
            string clientId,
            int pageSize,
            int page,
            CancellationToken cancellationToken = default)
        {
            using var scope = scopeService.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthServerDbContext>();
            var origins = await dbContext.CorsConfig
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => c.Origin)
                .ToPaginatedListAsync(pageSize, page, false, cancellationToken);
            return origins.Items.Select(o => new Uri(o)).ToPaginatedList(origins.Pagination);
        }

        public async Task<bool> IsOriginAllowedAsync(
            string origin,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(origin))
            {
                return false;
            }

            using var scope = scopeService.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthServerDbContext>();
            return await dbContext.CorsConfig
                .AsNoTracking()
                .Where(c => c.Origin == origin)
                .AnyAsync();
        }

        public async Task<CorsPolicy> ResolvePolicyAsync(HttpContext httpContext, string origin)
        {
            if (!await ResolvePolicyAsync_IsAllowed(httpContext, origin))
            {
                return null;
            }

            return new CorsPolicyBuilder(origin)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .Build();
        }

        private async Task<bool> ResolvePolicyAsync_IsAllowed(
            HttpContext httpContext,
            string origin)
        {
            if (httpContext.Request.Path.Value.Contains("api/connect/authorize-challenge-verify"))
            {
                return origin == webServerOptions.Endpoint;
            }

            return await IsOriginAllowedAsync(origin);
        }
        #endregion
    }
}
