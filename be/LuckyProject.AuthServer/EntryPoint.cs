using LuckyProject.AuthServer.Constants;
using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Hubs;
using LuckyProject.AuthServer.Middleware;
using LuckyProject.AuthServer.Models;
using LuckyProject.AuthServer.Services;
using LuckyProject.AuthServer.Services.Cors;
using LuckyProject.AuthServer.Services.Init;
using LuckyProject.AuthServer.Services.OpenId;
using LuckyProject.AuthServer.Services.Permissions;
using LuckyProject.AuthServer.Services.Roles;
using LuckyProject.AuthServer.Services.Sessions;
using LuckyProject.AuthServer.Services.Users;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Hosting.EntryPoint;
using LuckyProject.Lib.Swagger.Extensions;
using LuckyProject.Lib.Telegram.Extensions;
using LuckyProject.Lib.Web.Constants;
using LuckyProject.Lib.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OpenIddict.Abstractions;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using LuckyProject.AuthServer.Services.LpApi;

namespace LuckyProject.AuthServer
{
    internal class EntryPoint : AbstractWebHostEntryPoint, IEntryPoint
    {
        #region Internals & ctor
        private WebServerOptions webServerOptions;
        private LpSwaggerOptions swaggerOptions;

        public EntryPoint(string[] args) : base(args)
        { }
        #endregion

        #region Public interface

        public override Task ConfigureAsync()
        {
            ConfigureConfiguration();
            ConfigureWebServerOptions();
            ConfigureSwaggerOptions();
            ConfigureLogging();
            ConfigureServices();
            ConfigureWebServer();
            ConfigureHostedService();
            BuildApp();
            ConfigureApp();
            return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info("Runninng application...");
                await App.RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in application");
            }
        }
        #endregion

        #region Internals
        private void ConfigureConfiguration()
        {
            AppBuilder.Configuration.AddJsonFile("appsecret.json");
        }

        private void ConfigureWebServerOptions()
        {
            var configuration = AppBuilder.Configuration;
            var publicName = configuration.GetValue<string>("Application:WebServer:PublicName");
            var port = configuration.GetValue<int>("Application:WebServer:Port");
            var endpoint = $"https://{publicName}";
            if (port != 443)
            {
                endpoint += $":{port}";
            }
            var cert = new X509Certificate2(configuration.GetValue<string>(
                "Application:WebServer:CertPath"));
            var lpApiOrigins = configuration
                .GetSection("Application:WebServer:LpApiOrigins")
                .Get<HashSet<string>>();

            webServerOptions = new()
            {
                PublicName = publicName,
                Port = port,
                Endpoint = endpoint,
                Cert = cert,
            };

            AppBuilder.Services.AddOptions<WebServerOptions>().Configure(o =>
            {
                o.PublicName = webServerOptions.PublicName;
                o.Port = webServerOptions.Port;
                o.Endpoint = webServerOptions.Endpoint;
                o.Cert = webServerOptions.Cert;
            });
        }

        private void ConfigureSwaggerOptions()
        {
            if (!AppBuilder.Environment.IsLpDevelopment())
            {
                return;
            }

            var xmlDocs = Directory.EnumerateFiles(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "*.xml")
                .ToList();
            swaggerOptions = new()
            {
                Title = "LP AuthServer API",
                Description = "Lucky Project AuthServer API",
                Version = "baseline",
                XmlDocs = xmlDocs,
                AuthServerEndpoint = webServerOptions.Endpoint,
                SwaggerClientId = $"{LpWebConstants.ApiClients.WebPrefix}lp-swagger"
            };
        }

        #region ConfigureLogging
        private void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logLayoutFormat =
                "${level:format=TriLetter:uppercase=true}|${longdate}|${logger}${newline}" +
                "    [${scopenested:separator=>}]${newline}" +
                "    ${scopeindent}${message} ${exception:format=tostring}";
            var spamLoggers = new List<string>
            {
                "Microsoft.Hosting.*",
                "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware",
                "Quartz.*"
            };

            ConfigureLogging_AddFileLogger(config, logLayoutFormat, spamLoggers);
            ConfigureLogging_AddConsoleLogger(config, logLayoutFormat, spamLoggers);
            ConfigureLogging_Finish(config);
        }

        private void ConfigureLogging_AddFileLogger(
            LoggingConfiguration config,
            string logLayoutFormat,
            List<string> spamLoggers)
        {
            var target = new NLog.Targets.FileTarget("file")
            {
                FileName = "logfile.log",
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            ConfigureLogging_SwallowSpamLoggers(config, spamLoggers, target);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);
        }

        private void ConfigureLogging_AddConsoleLogger(
            LoggingConfiguration config,
            string logLayoutFormat,
            List<string> spamLoggers)
        {
            var target = new NLog.Targets.ColoredConsoleTarget("console")
            {
                Layout = new NLog.Layouts.SimpleLayout(logLayoutFormat)
            };
            ConfigureLogging_SwallowSpamLoggers(config, spamLoggers, target);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, target);
        }

        private void ConfigureLogging_SwallowSpamLoggers(
            LoggingConfiguration config,
            List<string> spamLoggers,
            Target target)
        {
            foreach(var spamLogger  in spamLoggers)
            {
                config.AddRule(new LoggingRule(spamLogger, NLog.LogLevel.Off, target)
                {
                    FinalMinLevel = NLog.LogLevel.Off
                });
            }
        }
        private void ConfigureLogging_Finish(LoggingConfiguration config)
        {
            NLog.LogManager.Configuration = config;
            AppBuilder.Logging.ClearProviders();
            AppBuilder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            AppBuilder.Logging.AddNLog(config);
        }
        #endregion

        private void ConfigureServices()
        {
            var configuration = AppBuilder.Configuration;
            var services = AppBuilder.Services;

            services.AddOptions<InitialSeedOptions>()
                .Bind(configuration.GetSection("Secrets:InitialSeed"));
            services.AddOptions<UsersLoginServiceSecrets>()
                .Bind(configuration.GetSection("Secrets:UsersLogin"));

            services.AddOptions<AuthServerDbContextOptions>()
                .Bind(configuration.GetSection("Application:Db"));
            services.AddOptions<LocalizationOptions>()
                .Bind(configuration.GetSection("Application:Localization"));

            services.AddLpBasicServices();
            services.AddLpAppVersionService(configuration.GetSection("Application:Version"));
            services.AddLpAppLocalizationService(
                new() { configuration.GetValue<string>("Application:Localization:I18nDir") });
            services.AddLpWebCommonServices();
            services.AddLpTelegramServices();

            services.AddDbContext<AuthServerDbContext>();
            services.AddTransient<IDbLayerInitService, DbLayerInitService>();
            ConfigureServices_Auth();
            services.AddLpWebCorsDynamicPolicy<AuthServerCorsService>(
            [
                webServerOptions.Endpoint
            ]);
            services.AddSingleton<IAuthServerCorsService, AuthServerCorsService>();

            services.AddScoped<IInitialSeedReader, InitialSeedReader>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddSingleton<IUsersLoginService, UsersLoginService>();
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddScoped<IOpenIdService, OpenIdService>();
            services.AddScoped<ILpApiService, LpApiService>();

            services.AddOptions<AuthSessionOptions>()
                .Bind(configuration.GetSection("Application:Sessions"));
            services.AddSingleton<IAuthSessionService, AuthSessionService>();
            services.AddSingleton<IAuthSessionManager, AuthSessionManager>();

            ConfigureServices_RequestTimeouts(services);
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            });
            services.AddSignalR()
                .AddNewtonsoftJsonProtocol(options =>
                {
                    options.PayloadSerializerSettings.Converters.Add(new IsoDateTimeConverter());
                    options.PayloadSerializerSettings.DateTimeZoneHandling =
                        DateTimeZoneHandling.Unspecified;
                    options.PayloadSerializerSettings.DateParseHandling =
                        DateParseHandling.DateTimeOffset;
                });
            services.AddHttpClient(LpWebConstants.Internals.HttpClients.ApiCallback);
            if (AppBuilder.Environment.IsLpDevelopment())
            {
                services.AddLpSwagger(swaggerOptions);
            }
        }

        private void ConfigureServices_Auth()
        {
            var configuration = AppBuilder.Configuration;
            var services = AppBuilder.Services;

            services.AddIdentity<AuthServerUser, AuthServerRole>(options =>
            {
                options.User = new()
                {
                    RequireUniqueEmail = false,
                    AllowedUserNameCharacters =
                        "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890-_."
                };
                options.Password = new()
                {
                    RequiredLength = 8,
                    RequireLowercase = true,
                    RequireUppercase = false,
                    RequireDigit = true,
                    RequireNonAlphanumeric = true,
                    RequiredUniqueChars = 1
                };
            })
                .AddEntityFrameworkStores<AuthServerDbContext>()
                .AddDefaultTokenProviders();

            services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore().UseDbContext<AuthServerDbContext>();
                    options.UseQuartz();
                })
                .AddServer(options =>
                {
                    options.SetAuthorizationEndpointUris(
                        "/api/connect/authorize-start",
                        "/api/connect/authorize-finish")
                        .SetLogoutEndpointUris(
                            "/api/connect/logout-start",
                            "/api/connect/logout-finish")
                        .SetTokenEndpointUris("/api/connect/token");

                    options.RegisterScopes(
                        OpenIddictConstants.Scopes.Profile,
                        OpenIddictConstants.Scopes.Email,
                        OpenIddictConstants.Scopes.Phone,
                        OpenIddictConstants.Scopes.Roles,
                        OpenIddictConstants.Scopes.OfflineAccess);

                    options.AllowAuthorizationCodeFlow()
                        .AllowClientCredentialsFlow()
                        .AllowRefreshTokenFlow();

                    options.AddEncryptionCertificate(webServerOptions.Cert)
                        .AddSigningCertificate(webServerOptions.Cert);

                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableTokenEndpointPassthrough();
                })
                .AddValidation(options =>
                {
                    options.SetIssuer(webServerOptions.Endpoint);
                    options.AddAudiences(LpWebConstants.Auth.Jwt.ApiAudience);
                    options.AddEncryptionCertificate(webServerOptions.Cert);
                    options.AddSigningCertificate(webServerOptions.Cert);
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });

            services.AddLpWebAuthenticationAuthorization
                <AuthServerJwtAuthorizationAuthenticationHandler>(
                    webServerOptions.Endpoint,
                    webServerOptions.Cert);
        }

        private void ConfigureServices_RequestTimeouts(IServiceCollection services)
        {
            AppBuilder.Services.AddRequestTimeouts(options =>
            {
                options.DefaultPolicy = new RequestTimeoutPolicy
                {
                    Timeout = AuthServerContants.TimeoutPolicies.Default.Timeout
                };
                options.AddPolicy(AuthServerContants.TimeoutPolicies.Long.Name,
                    new RequestTimeoutPolicy
                    {
                        Timeout = AuthServerContants.TimeoutPolicies.Long.Timeout
                    });
            });
        }

        private void ConfigureWebServer()
        {
            AppBuilder.WebHost.UseKestrel((context, serverOptions) =>
            {
                serverOptions.ListenAnyIP(webServerOptions.Port, listenOptions =>
                {
                    listenOptions.UseHttps(webServerOptions.Cert);
                });
            });
        }

        private void ConfigureHostedService()
        {
            AppBuilder.Services.AddHostedService<AuthServerService>();
        }

        private void ConfigureApp()
        {
            App.UseLpWebExceptionHandler(); 
            App.UseRequestTimeouts();
            App.UseRouting();
            App.UseLpWebCorsDynamicPolicy();
            App.UseStaticFiles();
            App.UseLpWebAuthenticationAuthorization();
            App.MapControllers();
            App.MapHub<NotificationsHub>("/notifications");
            if (App.Environment.IsLpDevelopment())
            {
                App.UseLpSwagger(swaggerOptions);
            }
        }
        #endregion
    }
}
