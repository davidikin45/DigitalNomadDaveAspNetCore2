﻿using AspnetCore.Base.Validation.Errors;
using AspNetCore.Base.ApiClient;
using AspNetCore.Base.Authentication;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.Cqrs;
using AspNetCore.Base.DependencyInjection.Modules;
using AspNetCore.Base.ElasticSearch;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Filters;
using AspNetCore.Base.Hangfire;
using AspNetCore.Base.HostedServices;
using AspNetCore.Base.Hosting;
using AspNetCore.Base.IntegrationEvents;
using AspNetCore.Base.Localization;
using AspNetCore.Base.Middleware;
using AspNetCore.Base.MiniProfiler;
using AspNetCore.Base.ModelBinders;
using AspNetCore.Base.MultiTenancy;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.MvcServices;
using AspNetCore.Base.Notifications;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Routing;
using AspNetCore.Base.Routing.Constraints;
using AspNetCore.Base.Security;
using AspNetCore.Base.Settings;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Swagger;
using AspNetCore.Base.Tasks;
using AspNetCore.Base.Validation.Providers;
using AspNetCore.Mvc.Extensions;
using AspNetCore.Mvc.Extensions.Conventions.Display;
using AspNetCore.Mvc.MvcAsApi.Conventions;
using AspNetCore.Mvc.MvcAsApi.Extensions;
using AspNetCore.Mvc.MvcAsApi.Middleware;
using AspNetCore.Mvc.MvcAsApi.ModelBinding;
using AspNetCoreRateLimit;
using Autofac;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Westwind.AspNetCore.LiveReload;
using static AspNetCore.Base.Localization.LocalizationExtensions;

namespace AspNetCore.Base
{
    public abstract class AppStartup
    {
        //https://github.com/aspnet/Extensions/issues/1096
        //.NET Core 3.0. IHostingEnvironment and IConfiguration only
        public AppStartup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Logger = loggerFactory.CreateLogger("Startup");

            Configuration = configuration;
            AppSettings = GetSettings<AppSettings>("AppSettings");

            HostingEnvironment = hostingEnvironment;

            //http://blog.hostforlife.eu/variables-and-configuration-in-asp-net-core-apps/
            //http://www.hishambinateya.com/goodbye-platform-abstractions
            //var workingDirectory = Directory.GetCurrentDirectory();
            WorkingDirectory = hostingEnvironment.ContentRootPath;

            //AppDomain.CurrentDomain.BaseDirectory
            //Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath)
            BinPath = AppContext.BaseDirectory;
            Logger.LogInformation($"Bin Folder: {BinPath}");

            PluginsPath = Path.Combine(BinPath, PluginsFolder);
            Logger.LogInformation($"Plugins Folder: {PluginsPath}");
            if (!Directory.Exists(PluginsPath)) Directory.CreateDirectory(PluginsPath);

            //Logs should always be relative to the Working Directory. Thats how serilog works.
            LogsPath = Path.Combine(WorkingDirectory, LogsFolder);
            Logger.LogInformation($"Logs Folder: {LogsPath}");
            if (!Directory.Exists(LogsPath)) Directory.CreateDirectory(LogsPath);

            //Data should generally be relative to the Working Directory.
            DataPath = Path.Combine(WorkingDirectory, DataFolder);
            Logger.LogInformation($"Data Folder: {DataPath}");
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);

            BinDataPath = Path.Combine(BinPath, DataFolder);
            Logger.LogInformation($"Bin Data Folder: {BinDataPath}");
            if (!Directory.Exists(BinDataPath)) Directory.CreateDirectory(BinDataPath);

            foreach (var publicUploadFolder in AppSettings.PublicUploadFolders.Split(','))
            {
                var path = HostingEnvironment.WebRootPath + publicUploadFolder;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            Logger.LogInformation($"Content Root Path (Working Directory): {hostingEnvironment.ContentRootPath}");
            Logger.LogInformation($"Web Root Path: {hostingEnvironment.WebRootPath}");

            AssemblyName = this.GetType().Assembly.GetName().Name;
            AppAssemblyPrefix = configuration.GetValue<string>("AppSettings:AssemblyPrefix");

            AssemblyBoolFilter = (a => a.FullName.Contains(AppAssemblyPrefix) || a.FullName.Contains(CommonAssemblyPrefix));
            AssemblyStringFilter = (s => (new FileInfo(s)).Name.Contains(AppAssemblyPrefix) || (new FileInfo(s)).Name.Contains(CommonAssemblyPrefix));

            //Load Assemblies into current AppDomain
            List<Assembly> binAssemblies = new List<Assembly>();
            foreach (var assemblyPath in Directory.EnumerateFiles(BinPath, "*.*", SearchOption.TopDirectoryOnly)
              .Where(file => new[] { ".dll" }.Any(file.ToLower().EndsWith))
              .Where(AssemblyStringFilter))
            {
                Logger.LogInformation($"Loading Assembly: {assemblyPath}");
                var assembly = Assembly.LoadFrom(assemblyPath);
                binAssemblies.Add(assembly);
            }

            //Load plugins into current AppDomain
            List<Assembly> pluginAssemblies = new List<Assembly>();
            foreach (var assemblyPath in Directory.EnumerateFiles(PluginsPath, "*.*", SearchOption.TopDirectoryOnly)
                               .Where(file => new[] { ".dll" }.Any(file.ToLower().EndsWith)))
            {
                Logger.LogInformation($"Loading Plugin: {assemblyPath}");
                var assembly = Assembly.LoadFrom(assemblyPath);
                pluginAssemblies.Add(assembly);
            }

            ApplicationParts = binAssemblies.Concat(pluginAssemblies).Where(AssemblyBoolFilter).ToList();
        }

        public ILogger Logger { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }

        public AppSettings AppSettings { get; }

        public string BinPath { get; }
        public string PluginsPath { get; }
        public string WorkingDirectory { get; }

        public string LogsPath { get; }
        public string DataPath { get; }
        public string BinDataPath { get; }

        public string AssemblyName { get; }
        public string AppAssemblyPrefix { get; }
        public string CommonAssemblyPrefix { get; } = "AspNetCore.Base";

        public string PluginsFolder { get; } = @"plugins\";
        public string LogsFolder { get; } = @"logs\";
        public string DataFolder { get; } = @"data\";
        public string AssetsFolder { get; } = "/files";

        public Func<Assembly, Boolean> AssemblyBoolFilter { get; }
        public Func<string, Boolean> AssemblyStringFilter { get; }

        public List<Assembly> ApplicationParts { get; }

        #region 1. Configure Services
        // Add services to the collection. Don't build or return
        // any IServiceProvider or the ConfigureContainer method
        // won't get called.
        //Disposal: https://stackoverflow.com/questions/40844151/when-are-net-core-dependency-injected-instances-disposed
        //If there are multiple registrations for the same service type the last registered type wins.
        public virtual void ConfigureServices(IServiceCollection services)
        {

            ConfigureSettingsServices(services);
            ConfigureDatabaseServices(services);
            ConfigureAuthenticationServices(services);

            //This will override defaultauthentication
            ConfigureIdentityServices(services);

            ConfigureAuthorizationServices(services);
            ConfigureSecurityServices(services);
            ConfigureEmailServices(services);
            ConfigureCachingServices(services);
            ConfigureResponseCompressionServices(services);
            ConfigureLocalizationServices(services);
            ConfigureMvcServices(services);

            ConfigureEventServices(services);
            ConfigureSignalRServices(services);
            ConfigureApiServices(services);
            ConfigureHttpClients(services);
            ConfigureProfilingServices(services);
            ConfigureHostedServices(services);
            ConfigureLiveReloadServices(services);

            AddHangfireJobServices(services);
        }

        #region Settings
        public virtual void ConfigureSettingsServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Settings");

            services.AddSingleton(Configuration);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AppSettings>>().Value);


            services.Configure<LocalizationSettings>(Configuration.GetSection("LocalizationSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<LocalizationSettings>>().Value);

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            services.PostConfigure<ConnectionStrings>(options =>
            {
                ManipulateConnectionStrings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<ConnectionStrings>>().Value);

            services.Configure<ServerSettings>(Configuration.GetSection("ServerSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<ServerSettings>>().Value);

            services.Configure<ApiClientSettings>(Configuration.GetSection("ApiClientSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<ApiClientSettings>>().Value);

            services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));
            services.PostConfigure<TokenSettings>(options =>
            {
                ManipulateTokenSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<TokenSettings>>().Value);

            services.Configure<CORSSettings>(Configuration.GetSection("CORSSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<CORSSettings>>().Value);

            services.Configure<PasswordSettings>(Configuration.GetSection("PasswordSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<PasswordSettings>>().Value);

            services.Configure<UserSettings>(Configuration.GetSection("UserSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<UserSettings>>().Value);

            services.Configure<AuthenticationSettings>(Configuration.GetSection("AuthenticationSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AuthenticationSettings>>().Value);

            services.Configure<AuthorizationSettings>(Configuration.GetSection("AuthorizationSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AuthorizationSettings>>().Value);

            services.Configure<CacheSettings>(Configuration.GetSection("CacheSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<CacheSettings>>().Value);

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.PostConfigure<EmailSettings>(options =>
            {
                ManipulateEmailSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<EmailSettings>>().Value);

            services.Configure<EmailTemplates>(Configuration.GetSection("EmailTemplates"));
            services.PostConfigure<EmailTemplates>(options =>
            {
                ManipluateEmailTemplateSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<EmailTemplates>>().Value);

            services.Configure<ElasticSettings>(Configuration.GetSection("ElasticSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<ElasticSettings>>().Value);

            services.Configure<SwitchSettings>(Configuration.GetSection("SwitchSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<SwitchSettings>>().Value);

            services.Configure<AssemblyProviderOptions>(options =>
            {
                options.BinPath = BinPath;
                options.AssemblyFilter = AssemblyStringFilter;
            });

            services.AddTransient(sp => sp.GetService<IOptions<AssemblyProviderOptions>>().Value);
        }

        //By default SQL and SQLite databases are created within the Working Directory = Directory.GetCurrentDirectory()
        //%BINDATA% creates the SQL and SQLite databases within the Bin Directory = AppContext.BaseDirectory
        private ConnectionStrings ManipulateConnectionStrings(ConnectionStrings options)
        {
            var keys = options.Keys.ToList();

            foreach (var key in keys)
            {
                if (options[key].Contains("%DATA%"))
                {
                    options[key] = options[key].Replace("%DATA%", DataPath);
                }

                if (options[key].Contains("%BINDATA%"))
                {
                    options[key] = options[key].Replace("%BINDATA%", BinDataPath);
                }

                if (options[key].Contains("%BIN%"))
                {
                    options[key] = options[key].Replace("%BIN%", BinPath);
                }
            }

            return options;
        }

        private TokenSettings ManipulateTokenSettings(TokenSettings options)
        {
            if (!string.IsNullOrEmpty(options.PrivateKeyPath))
            {
                options.PrivateKeyPath = HostingEnvironment.MapContentPath(options.PrivateKeyPath);
            }

            if (!string.IsNullOrEmpty(options.PublicKeyPath))
            {
                options.PublicKeyPath = HostingEnvironment.MapContentPath(options.PublicKeyPath);
            }

            if (!string.IsNullOrEmpty(options.PrivateCertificatePath))
            {
                options.PrivateCertificatePath = HostingEnvironment.MapContentPath(options.PrivateCertificatePath);
            }

            if (!string.IsNullOrEmpty(options.PublicCertificatePath))
            {
                options.PublicCertificatePath = HostingEnvironment.MapContentPath(options.PublicCertificatePath);
            }

            return options;
        }

        private void ManipluateEmailTemplateSettings(EmailTemplates options)
        {
            if (!string.IsNullOrEmpty(options.Welcome))
            {
                options.Welcome = HostingEnvironment.MapContentPath(options.Welcome);
            }

            if (!string.IsNullOrEmpty(options.ResetPassword))
            {
                options.ResetPassword = HostingEnvironment.MapContentPath(options.ResetPassword);
            }
        }

        private void ManipulateEmailSettings(EmailSettings options)
        {
            if (!options.FileSystemFolder.Contains(@":\"))
            {
                options.FileSystemFolder = Path.Combine(WorkingDirectory, options.FileSystemFolder);
            }
        }

        public TSettings GetSettings<TSettings>(string sectionKey) where TSettings : class
        {
            var settingsSection = Configuration.GetSection(sectionKey);
            return settingsSection.Get<TSettings>();
        }

        #endregion

        #region Database
        public virtual void ConfigureDatabaseServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Databases");

            var connectionStrings = ManipulateConnectionStrings(GetSettings<ConnectionStrings>("ConnectionStrings"));

            var tenantsConnectionString = connectionStrings.ContainsKey("TenantConnection") ? connectionStrings["TenantConnection"] : null;
            var identityConnectionString = connectionStrings.ContainsKey("IdentityConnection") ? connectionStrings["IdentityConnection"] : null;
            var defaultConnectionString = connectionStrings.ContainsKey("DefaultConnection") ? connectionStrings["DefaultConnection"] : null;
            //Configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "HangfireConnection") ? Configuration.GetConnectionString("HangfireConnection").Replace("%BIN%", BinPath).Replace("%DATA%", DataPath) : null;
            var hangfireConnectionString = connectionStrings.ContainsKey("HangfireConnection") ? connectionStrings["HangfireConnection"] : null;

            AddDatabases(services, connectionStrings, tenantsConnectionString, identityConnectionString, hangfireConnectionString, defaultConnectionString);
            AddUnitOfWorks(services);

            services.AddHangfire(hangfireConnectionString, false);
        }
        #endregion

        #region Authentication
        public virtual void ConfigureAuthenticationServices(IServiceCollection services)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x?view=aspnetcore-2.1#cookie-based-authentication
            //Define a default scheme in 2.0 if one of the following conditions is true:
            //You use the [Authorize] attribute or authorization policies without specifying schemes

            //IdentityConstants.ApplicationScheme = "Identity.Application"
            //CookieAuthenticationDefaults.AuthenticationScheme = "Cookies"
            //JwtBearerDefaults.AuthenticationScheme = "Bearer"

            Logger.LogInformation("Configuring Authentication");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            var authenticationSettings = GetSettings<AuthenticationSettings>("AuthenticationSettings");
            var tokenSettings = ManipulateTokenSettings(GetSettings<TokenSettings>("TokenSettings"));

            if (authenticationSettings.Application.Enable)
            {
                Logger.LogInformation("Configuring Cookie Authentication");

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = appSettings.CookieAuthName;
                });
            }

            services.ConfigureExternalCookie(options =>
            {
                options.Cookie.Name = appSettings.CookieExternalAuthName;
            });

            var authenticationBuilder = services.AddAuthentication();

            if (authenticationSettings.JwtToken.Enable)
            {
                Logger.LogInformation($"Configuring JWT Authentication" + Environment.NewLine +
                                       $"Key:{tokenSettings.Key ?? ""}" + Environment.NewLine +
                                       $"PublicKeyPath: {tokenSettings.PublicKeyPath ?? ""}" + Environment.NewLine +
                                       $"PublicCertificatePath: {tokenSettings.PublicCertificatePath ?? ""}" + Environment.NewLine +
                                       $"ExternalIssuers: {tokenSettings.ExternalIssuers ?? ""}" + Environment.NewLine +
                                       $"LocalIssuer: {tokenSettings.LocalIssuer ?? ""}" + Environment.NewLine +
                                       $"Audiences: {tokenSettings.Audiences ?? ""}");

                services.Configure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                });

                //https://wildermuth.com/2017/08/19/Two-AuthorizationSchemes-in-ASP-NET-Core-2
                authenticationBuilder.AddJwtAuthentication(
                   tokenSettings.Key,
                   tokenSettings.PublicKeyPath,
                   tokenSettings.PublicCertificatePath,
                   tokenSettings.ExternalIssuers,
                   tokenSettings.LocalIssuer,
                   tokenSettings.Audiences);
            }

            if (authenticationSettings.OpenIdConnectJwtToken.Enable)
            {
                Logger.LogInformation("Configuring IdentityServer JWT Authentication");

                //scheme
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44318/";
                    options.ApiName = "api";
                    options.ApiSecret = "apisecret"; //Only need this if AccessTokenType = AccessTokenType.Reference
                });
            }

            if (authenticationSettings.OpenIdConnect.Enable)
            {
                Logger.LogInformation("Configuring OpenIdConnect");

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types
                services.Configure<AuthenticationOptions>(options =>
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
                    //overides "Identity.Application"/IdentityConstants.ApplicationScheme set by AddIdentity
                    //Use cookie authentication without ASP.NET Core Identity
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Challenge scheme is how user should login if they arent already logged in.
                });

                //authetication scheme seperate to application cookie
                //Use cookie authentication without ASP.NET Core Identity
                authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                {
                    options.Cookie.Name = appSettings.CookieAuthName;
                    options.AccessDeniedPath = "Authorization/AccessDenied";
                });

                authenticationBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44318";

                    //options.ResponseType = "code"; //Authorization
                    //options.ResponseType = "id_token"; //Implicit
                    //options.ResponseType = "id_token token"; //Implicit
                    options.ResponseType = "code id_token"; //Hybrid
                    //options.ResponseType = "code token"; //Hybrid
                    //options.ResponseType = "code id_token token"; //Hybrid

                    //options.CallbackPath = new PathString("...")
                    //options.SignedOutCallbackPath = new PathString("...")
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    options.Scope.Add("api");
                    options.Scope.Add("subscriptionlevel");
                    options.Scope.Add("country");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;

                    options.ClientId = "mvc_client";
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("idp");

                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                    options.ClaimActions.MapUniqueJsonKey("country", "country");

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = "given_name",
                        RoleClaimType = "role"
                    };
                });
            }

            if (authenticationSettings.Google.Enable)
            {
                authenticationBuilder.AddGoogle("Google", options =>
                {
                    options.ClientId = authenticationSettings.Google.ClientId;
                    options.ClientSecret = authenticationSettings.Google.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            if (authenticationSettings.Facebook.Enable)
            {
                authenticationBuilder.AddFacebook("Facebook", options =>
                {
                    options.ClientId = authenticationSettings.Facebook.ClientId;
                    options.ClientSecret = authenticationSettings.Facebook.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }
        }
        #endregion

        #region Authorization
        public virtual void ConfigureAuthorizationServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Authorization");

            var authorizationSettings = GetSettings<AuthorizationSettings>("AuthorizationSettings");

            //Add this to controller or action using Authorize(Policy = "UserMustBeAdmin")
            //Can create custom requirements by implementing IAuthorizationRequirement and AuthorizationHandler (Needs to be added to services as scoped)
            services.AddAuthorization(options =>
            {

                //https://ondrejbalas.com/authorization-options-in-asp-net-core/
                //The default policy will only be executed on requests prior to entering protected actions such as those wrapped by an [Authorize] attribute.
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

                options.AddPolicy("UserMustBeAdmin", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    //policyBuilder.AddRequirements();
                });
            });

            //https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.1&tabs=aspnetcore2x
            services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AnonymousAuthorizationHandler>();
            //Scope name as policy
            //https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }
        #endregion

        #region Security
        public virtual void ConfigureSecurityServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Security");

            var switchSettings = GetSettings<SwitchSettings>("SwitchSettings");
            var corsSettings = GetSettings<CORSSettings>("CORSSettings");

            if (switchSettings.EnableIFramesGlobal)
            {
                services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            }

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.ConfigureCorsAllowAnyOrigin("AllowAnyOrigin");
            services.ConfigureCorsAllowSpecificOrigin("AllowSpecificOrigin", corsSettings.Domains);
        }
        #endregion

        #region Email
        public virtual void ConfigureEmailServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Email");

            var emailSettings = GetSettings<EmailSettings>("EmailSettings");

            if (emailSettings.SendEmailsViaSmtp)
            {
                services.AddTransient<IEmailService, EmailServiceSmtp>();
            }
            else
            {
                services.AddTransient<IEmailService, EmailServiceFileSystem>();
            }
        }
        #endregion

        #region Caching
        public virtual void ConfigureCachingServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Caching");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            services.AddResponseCaching(options =>
            {
                options.SizeLimit = appSettings.ResponseCacheSizeMB * 1024 * 1024; //100Mb
                options.MaximumBodySize = 64 * 1024 * 1024; //64Mb
            });


            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            //https://stackoverflow.com/questions/46492736/asp-net-core-2-0-http-response-caching-middleware-nothing-cached
            //Client Side Cache Time
            //services.AddHttpCacheHeaders(opt => opt.MaxAge = 600, opt => opt.MustRevalidate = true);
            services.AddHttpCacheHeaders();
        }
        #endregion

        #region Compression
        public virtual void ConfigureResponseCompressionServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Response Compression");

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { ""});
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
        }
        #endregion

        #region Localization
        public virtual void ConfigureLocalizationServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Localization");

            //https://github.com/RickStrahl/Westwind.Globalization

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            //CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
            //Globalization(G11N): The process of making an app support different languages and regions.
            //Localization(L10N): The process of customizing an app for a given language and region.
            //Internationalization(I18N): Describes both globalization and localization.
            //Culture: It's a language and, optionally, a region.
            //Neutral culture: A culture that has a specified language, but not a region. (for example "en", "es")
            //                    Specific culture: A culture that has a specified language and region. (for example "en-US", "en-GB", "es-CL")
            //                Parent culture: The neutral culture that contains a specific culture. (for example, "en" is the parent culture of "en-US" and "en-GB")
            //            Locale: A locale is the same as a culture.
        }
        #endregion

        #region Mvc
        public virtual void ConfigureMvcServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Mvc");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            var localizationSettings = GetSettings<LocalizationSettings>("LocalizationSettings");
            var switchSettings = GetSettings<SwitchSettings>("SwitchSettings");
            var authorizationSettings = GetSettings<AuthorizationSettings>("AuthorizationSettings");

            //settings will automatically be used by JsonConvert.SerializeObject/DeserializeObject.
            var defaultSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                TypeNameHandling = TypeNameHandling.None
            };

            JsonConvert.DefaultSettings = () => defaultSettings;

            //https://www.strathweb.com/2018/04/generic-and-dynamically-generated-controllers-in-asp-net-core-mvc/

            //.NET Core 3.0
            //services.AddControllers(); //Controllers
            //services.AddRazorPages(); //Razor Pages, Included in AddMvc
            //services.AddControllersWithViews() //Controllers + Views, Included in AddMvc

            //.NET Core 3.0 
            //services.Configure<JsonOptions>(options =>
            //{
                //By default System.Text.Json is case sensitive. JSON.NET is default case insenstive.
                //options.SerializerOptions.WriteIndented = defaultSettings.Formatting == Formatting.Indented;
                //options.SerializerOptions.IgnoreNullValues = defaultSettings.DefaultValueHandling == DefaultValueHandling.Ignore;
                //options.SerializerOptions.IgnoreReadOnlyProperties = false;
                //options.SerializerOptions.PropertyNameCaseInsensitive = true;
                //options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                //options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //});


            var mvc = services.AddMvc(options =>
            {
                //Versioning Fix until .NET Core 3.0
                options.EnableEndpointRouting = false;

                //https://github.com/aspnet/Security/issues/1764
                options.AllowCombiningAuthorizeFilters = false;

                //Adds {culture=default} to ALL routes
                if (localizationSettings.AlwaysIncludeCultureInUrl)
                {
                    options.AddCultureRouteConvention();
                }
                else
                {
                    options.AddOptionalCultureRouteConvention();
                }

                options.Conventions.Add(new MvcAsApiConvention(o =>
                {
                    o.MvcErrorOptions = (mvcErrorOptions) =>
                    {

                    };
                    o.MvcExceptionOptions = (mvcExceptionOptions) =>
                    {
                        mvcExceptionOptions.ActionResultFactories.Add(typeof(UnauthorizedErrors), (context, exception, logger) =>
                        {
                            return context.HttpContext.User.Identity.IsAuthenticated ? new ForbidResult() : (IActionResult)new ChallengeResult();
                        });
                    };
                    o.ApiErrorOptions = (apiErrorOptions) =>
                    {

                    };
                    o.ApiExceptionOptions = (apiExceptionOptions) =>
                    {
                        apiExceptionOptions.ActionResultFactories.Add(typeof(UnauthorizedErrors), (context, exception, logger) =>
                        {
                            return context.HttpContext.User.Identity.IsAuthenticated ? new StatusCodeResult(StatusCodes.Status403Forbidden) : new UnauthorizedResult();
                        });
                    };
                }));


                //Middleware Pipeline - Wraps MVC
                options.Filters.Add(new MiddlewareFilterAttribute(typeof(LocalizationPipeline)));

                //Action Pipeline - Wraps Actions within MVC
                //options.Filters.Add<AutoValidateFormAntiforgeryTokenAttribute>();
                options.Filters.Add<ValidatableAttribute>();
                //options.Filters.Add<OperationCancelledExceptionFilterAttribute>();

                //[FromBody] or [ApiController]
                //https://stackoverflow.com/questions/31952002/asp-net-core-mvc-how-to-get-raw-json-bound-to-a-string-without-a-type
                //options.InputFormatters.Insert(0, new RawStringRequestBodyInputFormatter());
                options.InputFormatters.Insert(0, new RawBytesRequestBodyInputFormatter());

                //options.Filters.Add(typeof(ModelValidationFilter));
                ConfigureMvcCachingProfiles(options);
                ConfigureMvcVariableResourceRepresentations(options);

                if (authorizationSettings.UserMustBeAuthorizedByDefault)
                {
                    //https://ondrejbalas.com/authorization-options-in-asp-net-core/
                    //The authorization filter will be executed on any request that enters the MVC middleware and maps to a valid action.
                    var globalPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                    options.Filters.Add(new AuthorizeFilter(globalPolicy));
                }
            })
            .AddXmlSerializerFormatters() //XML Opt out. Contract Serializer is Opt in
            .AddJsonOptions(options =>
            {
                //https://github.com/aspnet/Mvc/blob/32e21e2a5c63e20bd62b9c1699932207b962fc50/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonSerializerSettingsProvider.cs#L31-L41
                options.SerializerSettings.ReferenceLoopHandling = defaultSettings.ReferenceLoopHandling;
                options.SerializerSettings.Formatting = defaultSettings.Formatting;
                options.SerializerSettings.Converters = defaultSettings.Converters;
                options.SerializerSettings.ContractResolver = defaultSettings.ContractResolver;
                options.SerializerSettings.DefaultValueHandling = defaultSettings.DefaultValueHandling;
                options.SerializerSettings.NullValueHandling = defaultSettings.NullValueHandling;
                options.SerializerSettings.MissingMemberHandling = defaultSettings.MissingMemberHandling;
                options.SerializerSettings.TypeNameHandling = defaultSettings.TypeNameHandling;
            })
            //.NET Core 3.0
            //This sets up MVC and configures it to use Json.NET instead of that new API(System.Text.Json). AddNewtonsoftJson method has an overload that allows you to configure the Json.NET options like you were used to with AddJsonOptions in ASP.NET Core 2.x
            //.AddNewtonsoftJson(options =>
            //{
            //    //using Microsoft.AspNetCore.Mvc.NewtonsoftJson
            //    //https://github.com/aspnet/Mvc/blob/32e21e2a5c63e20bd62b9c1699932207b962fc50/src/Microsoft.AspNetCore.Mvc.Formatters.Json/JsonSerializerSettingsProvider.cs#L31-L41
            //    options.SerializerSettings.ReferenceLoopHandling = defaultSettings.ReferenceLoopHandling;
            //    options.SerializerSettings.Formatting = defaultSettings.Formatting;
            //    options.SerializerSettings.Converters = defaultSettings.Converters;
            //    options.SerializerSettings.ContractResolver = defaultSettings.ContractResolver;
            //    options.SerializerSettings.DefaultValueHandling = defaultSettings.DefaultValueHandling;
            //    options.SerializerSettings.NullValueHandling = defaultSettings.NullValueHandling;
            //    options.SerializerSettings.MissingMemberHandling = defaultSettings.MissingMemberHandling;
            //    options.SerializerSettings.TypeNameHandling = defaultSettings.TypeNameHandling;
            //})
            .AddCookieTempDataProvider(options =>
            {
                // new API
                options.Cookie.Name = appSettings.CookieTempDataName;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            //By default, ASP.NET Core will resolve the controller parameters from the container but doesn’t actually resolve the controller from the container.
            //https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/
            //The AddControllersAsServices method does two things - it registers all of the Controllers in your application with the DI container as Transient (if they haven't already been registered) and replaces the IControllerActivator registration with the ServiceBasedControllerActivator
            .AddControllersAsServices()
            .AddPointModelBinder();
            //.NET Core 3.0 using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
            //.AddRazorRuntimeCompilation();
         

            services.AddMvcDisplayConventions(
            new AppendAsterixToRequiredFieldLabels((viewContext) => ((viewContext.ViewData.ContainsKey("EditMode") && (Boolean)viewContext.ViewData["EditMode"]) || (viewContext.ViewData.ContainsKey("CreateMode") && (Boolean)viewContext.ViewData["CreateMode"])) && !(viewContext.ViewData.ContainsKey("DetailsMode") && (Boolean)viewContext.ViewData["DetailsMode"])),
            new HtmlByNameConventionFilter(),
            new LabelTextConventionFilter(),
            new TextAreaByNameConventionFilter(),
            new TextboxPlaceholderConventionFilter());

            services.AddMvcValidationConventions();
            services.AddMvcDisplayAttributes();
            services.AddInheritanceValidationAttributeAdapterProvider();
            services.AddFluentMetadata();
            services.AddDynamicModelBinder();
            services.AddViewRenderer();

            services.AddRequestLocalizationOptions(
                localizationSettings.DefaultCulture,
                localizationSettings.SupportAllCountriesFormatting,
                localizationSettings.SupportAllLanguagesFormatting,
                localizationSettings.SupportUICultureFormatting,
                localizationSettings.SupportDefaultCultureLanguageFormatting,
                localizationSettings.SupportedUICultures);

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();

            services.AddCookiePolicy(appSettings.CookieConsentName);

            //singleton
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            //services.AddViewLocationExpander(appSettings.MvcImplementationFolder);

            var sharedViewFolders = new string[] {
                "Bundles",
                "Sidebar",
                "CRUD",
                "Navigation",
                "Footer",
                "Alerts",
                "CookieConsent",
            };

            // Views/{1} = {1}/Views
            // Shared = Shared/Views


            //Non Area
            //https://stackoverflow.com/questions/36747293/how-to-specify-the-view-location-in-asp-net-core-mvc-when-using-custom-locations
            services.Configure<RazorViewEngineOptions>(o =>
            {
                // {2} is area, {1} is controller,{0} is the action    
                //o.ViewLocationFormats.Clear();
                o.ViewLocationFormats.Add("/" + appSettings.MvcImplementationFolder + "{1}/Views/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/" + appSettings.MvcImplementationFolder + "Shared/Views/{0}" + RazorViewEngine.ViewExtension);

                foreach (var sharedViewFolder in sharedViewFolders)
                {
                    o.ViewLocationFormats.Add("/" + appSettings.MvcImplementationFolder + "Shared/Views/" + sharedViewFolder + "/{0}" + RazorViewEngine.ViewExtension);
                }
            });

            //Areas
            //https://stackoverflow.com/questions/36747293/how-to-specify-the-view-location-in-asp-net-core-mvc-when-using-custom-locations
            services.Configure<RazorViewEngineOptions>(o =>
            {
                // {2} is area, {1} is controller,{0} is the action    
                //o.AreaViewLocationFormats.Clear();
                o.AreaViewLocationFormats.Add("/Areas/{2}/" + appSettings.MvcImplementationFolder + "{1}/Views/{0}" + RazorViewEngine.ViewExtension);
                o.AreaViewLocationFormats.Add("/Areas/{2}/" + appSettings.MvcImplementationFolder + "Shared/Views/{0}" + RazorViewEngine.ViewExtension);

                foreach (var sharedViewFolder in sharedViewFolders)
                {
                    o.AreaViewLocationFormats.Add("/Areas/{2}/" + appSettings.MvcImplementationFolder + "Shared/Views/" + sharedViewFolder + "/{0}" + RazorViewEngine.ViewExtension);
                }

                o.AreaViewLocationFormats.Add("/Areas/Shared/Views/{0}" + RazorViewEngine.ViewExtension);

                foreach (var sharedViewFolder in sharedViewFolders)
                {
                    o.AreaViewLocationFormats.Add("/Areas/Shared/Views/" + sharedViewFolder + "/{0}" + RazorViewEngine.ViewExtension);
                }

                o.AreaViewLocationFormats.Add("/" + appSettings.MvcImplementationFolder + "Shared/Views/{0}" + RazorViewEngine.ViewExtension);

                foreach (var sharedViewFolder in sharedViewFolders)
                {
                    o.AreaViewLocationFormats.Add("/" + appSettings.MvcImplementationFolder + "Shared/Views/" + sharedViewFolder + "{0}" + RazorViewEngine.ViewExtension);
                }
            });

            services.AddSingleton<FeatureService>();
            services.AddSingleton<BundleConfigService>();
            services.AddSingleton<NavigationService>();

            ConfigureMvcModelValidation(services);
            ConfigureMvcApplicationParts(mvc, services);
            ConfigureMvcRouting(services);

        }

        public virtual void ConfigureMvcCachingProfiles(MvcOptions options)
        {
            //Cache-control: no-cache = store response on client browser but recheck with server each request 
            //Cache-control: no-store = dont store response on client
            options.CacheProfiles.Add("Cache24HourNoParams", new CacheProfile()
            {
                VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                //VaryByQueryKeys = "", Only used for server side caching
                Duration = 60 * 60 * 24, // 24 hour,
                Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                NoStore = false
            });

            options.CacheProfiles.Add("Cache24HourParams", new CacheProfile()
            {
                //IIS DynamicCompressionModule and StaticCompressionModule add the Accept-Encoding Vary header.
                VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                VaryByQueryKeys = new string[] { "*" }, //Only used for server side caching
                Duration = 60 * 60 * 24, // 24 hour,
                Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                NoStore = false
            });
        }

        public virtual void ConfigureMvcVariableResourceRepresentations(MvcOptions options)
        {
            //Accept = Response MIME type client is able to understand.
            //Accept-Language = Response Language client is able to understand.
            //Accept-Encoding = Response Compression client is able to understand.

            //Prevents returning object representation in default format when request format isn't available
            options.ReturnHttpNotAcceptable = true; //If Browser sends Accept not containing */* the server will try to find a formatter that can produce a response in one of the formats specified by the accept header.

            options.RespectBrowserAcceptHeader = false; //If Browser sends Accept containing */* the server will ignore Accept header and use the first formatter that can format the object.

            //Variable resource representations
            //Use RequestHeaderMatchesMediaTypeAttribute to route for Accept header. Version media types not URI!
            var jsonOutputFormatter = options.OutputFormatters
               .OfType<JsonOutputFormatter>().FirstOrDefault();

            if (jsonOutputFormatter != null)
            {

            }

            var jsonInputFormatter = options.InputFormatters
               .OfType<JsonInputFormatter>().FirstOrDefault();
            if (jsonInputFormatter != null)
            {

            }

            options.FormatterMappings.SetMediaTypeMappingForFormat(
                           "xml", "application/xml");
        }

        public virtual void ConfigureMvcRouting(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("promo", typeof(PromoConstraint));
                options.ConstraintMap.Add("tokenCheck", typeof(TokenConstraint));
                options.ConstraintMap.Add("versionCheck", typeof(RouteVersionConstraint));
                options.ConstraintMap.Add("cultureCheck", typeof(CultureConstraint));
            });
        }

        public virtual void ConfigureMvcModelValidation(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Mvc Model Validation");

            var switchSettings = GetSettings<SwitchSettings>("SwitchSettings");

            //Disable IObjectValidatable and Validation Attributes from being evaluated and populating modelstate
            //https://stackoverflow.com/questions/46374994/correct-way-to-disable-model-validation-in-asp-net-core-2-mvc
            if (!switchSettings.EnableMVCModelValidation)
            {
                var validator = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectModelValidator));
                if (validator != null)
                {
                    services.Remove(validator);
                    services.Add(new ServiceDescriptor(typeof(IObjectModelValidator), _ => new NonValidatingValidator(), ServiceLifetime.Singleton));
                }
            }
        }

        public virtual void ConfigureMvcApplicationParts(IMvcBuilder mvcBuilder, IServiceCollection services)
        {
            Logger.LogInformation("Configuring Application Parts");

            //Add Controllers from other assemblies
            foreach (var assembly in ApplicationParts)
            {
                mvcBuilder.AddApplicationPart(assembly).AddControllersAsServices();
            }

            //Add Embedded views from other assemblies
            services.Configure<RazorViewEngineOptions>(options =>
            {
                //Add Embedded Views from other assemblies
                //Edit and Continue wont work with these views.
                foreach (var assembly in ApplicationParts)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly));
                }
            });
        }
        #endregion

        #region Blazor
        public virtual void ConfigureBlazorServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Blazor");

            //services.AddServerSideBlazor();
        }
        #endregion

        #region Events
        public virtual void ConfigureEventServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Events");

            services.AddCqrs(ApplicationParts);
            services.AddHangFireDomainEvents(ApplicationParts);
            services.AddHangFireIntegrationEvents(ApplicationParts);
        }
        #endregion

        #region SignalR
        public virtual void ConfigureSignalRServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring SignalR");

            services.AddSignalR();
            //.NET Core 3.0
            //.AddJsonProtocol(options =>
            //{
            //    options.WriteIndented = defaultSettings.Formatting == Formatting.Indented;
            //    options.IgnoreNullValues = defaultSettings.DefaultValueHandling == DefaultValueHandling.Ignore;
            //    options.IgnoreReadOnlyProperties = false;
            //    options.PropertyNameCaseInsensitive = true;
            //    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //});
        }
        #endregion

        #region Api
        public virtual void ConfigureApiServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Api");

            var appSettings = GetSettings<AppSettings>("AppSettings");

            //Overrides the default IClientErrorFactory implementation which adds traceId, timeGenerated and exception details to the ProblemDetails response.
            services.AddProblemDetailsClientErrorAndExceptionFactory(options => options.ShowExceptionDetails = HostingEnvironment.IsDevelopment() || HostingEnvironment.IsIntegration());
            //Overrides the default InvalidModelStateResponseFactory, adds traceId and timeGenerated to the ProblemDetails response. 
            services.ConfigureProblemDetailsInvalidModelStateFactory(options => options.EnableAngularErrors = true);

            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";
                //Tells swagger to replace the version in the controller route
                setupAction.SubstituteApiVersionInUrl = true;
            });

            services.AddApiVersioning(option =>
            {
                //http://sundeepkamath.in/posts/rest-api-versioning-in-aspnet-core-part-1/
                //Query string parameter
                //URL path segment
                //HTTP header
                //Media type parameter

                option.ReportApiVersions = true;
                //Header then QueryString is consistent with how Accept header/FormatFilter works.
                option.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(), new MediaTypeApiVersionReader("v"), new HeaderApiVersionReader("api-version"), new QueryStringApiVersionReader("api-version", "v", "ver", "version"));
                //option.ApiVersionReader = new UrlSegmentApiVersionReader() /v{version:apiVersion}
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;

                //Add conventions
                //option.Conventions.Controller<>().HasApiVersion(new ApiVersion(1, 0));
            });

            //API rate limiting
            //services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 3,
                        Period = "5m"
                    },
                     new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 2,
                        Period = "10s"
                    }
                };
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddHealthChecks();

            string xmlDocumentationFileName = AssemblyName + ".xml";
            var xmlDocumentationPath = Path.Combine(BinPath, xmlDocumentationFileName);
            services.AddSwagger(appSettings.AssemblyPrefix + " API", "", "", "http://www.google.com", xmlDocumentationPath);
        }
        #endregion

        #region Elastic
        public virtual void ConfigureElasticServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring ElasticSearch");

            var elasticSettings = GetSettings<ElasticSettings>("ElasticSettings");
            if (!string.IsNullOrWhiteSpace(elasticSettings.Uri))
            {
                services.AddElasticSearch(elasticSettings.Uri, elasticSettings.DefaultIndex, elasticSettings.DefaultTypeName);
            }
        }
        #endregion

        #region GraphQL
        public virtual void ConfigureGraphQL(IServiceCollection services)
        {
            Logger.LogInformation("Configuring GraphQL");

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));

            services.AddGraphQL(o => { o.ExposeExceptions = true; })
              .AddGraphTypes(ServiceLifetime.Scoped)
              .AddUserContextBuilder(httpContext => httpContext.User)
              .AddDataLoader()
              .AddWebSockets();

        }
        #endregion

        #region HttpClients
        //https://www.codemag.com/article/1807041/What%E2%80%99s-New-in-ASP.NET-Core-2.1
        public virtual void ConfigureHttpClients(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Http Clients");

            services.AddTransient<ProfilingHttpHandler>();
            services.AddTransient<AuthorizationBearerProxyHttpHandler>();
            services.AddTransient<AuthorizationJwtProxyHttpHandler>();

            AddHttpClients(services);

            //JSON requests and responses are serialized/deserialized using Json.NET. By default will use the serializer settings that can be configured by setting Newtonsoft.Json.JsonConvert.DefaultSettings:

            //using Microsoft.Extensions.Http
            //When using typed client its best to put client config in the constructor.
            //services.AddHttpClient<IClient, Client>()
            //
            //    .AddHttpMessageHandler<ProfilingHttpHandler>()
            //    .AddHttpMessageHandler<AuthorizationProxyHttpHandler>()
            //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            //{
            //    AllowAutoRedirect = true,
            //    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            //});
            //services.AddHttpClient("name")
            //     .AddHttpMessageHandler<ProfilingHttpHandler>()
            //     .AddHttpMessageHandler<AuthorizationProxyHttpHandler>()
            //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            //{
            //    AllowAutoRedirect = true,
            //    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            //});

            // services.AddHttpClient(typeof(IClient).Name)
            // .ConfigureHttpClient(c =>
            // {
            //     c.BaseAddress = new Uri("http://localhost:5000");
            // })
            //.AddTypedClient<IClient>((httpClient, sp) =>
            // {
            //      //return implementation
            //      var defaultSettings = new JsonSerializerSettings()
            //     {
            //         ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //         Formatting = Formatting.Indented,
            //         Converters = new List<JsonConverter>() { new Newtonsoft.Json.Converters.StringEnumConverter() },
            //         ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //         DefaultValueHandling = DefaultValueHandling.Include,
            //         NullValueHandling = NullValueHandling.Include,
            //         MissingMemberHandling = MissingMemberHandling.Ignore,
            //         TypeNameHandling = TypeNameHandling.None
            //     };

            //     return new Client(httpClient, serializerSettings);
            //     return Refit.RestService.For<IClient>(c);
            // });

            //using Refit.HttpClientFactory
            //https://github.com/reactiveui/refit

            //var settings = new RefitSettings();
            //services.AddRefitClient<IClient, Client>(settings)
            //.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.example.com"));
        }

        public abstract void AddHttpClients(IServiceCollection services);
        #endregion

        #region Profiling
        //https://miniprofiler.com/dotnet/AspDotNetCore
        public virtual void ConfigureProfilingServices(IServiceCollection services)
        {
            services.AddMiniProfiler("", false);
        }
        #endregion

        #region Identity
        //https://www.codemag.com/article/1807041/What%E2%80%99s-New-in-ASP.NET-Core-2.1
        public virtual void ConfigureIdentityServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Identity");

        }
        #endregion

        #region Hosted Services
        public virtual void ConfigureHostedServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Hosted Services");

            //Inject IBackgroundTaskQueue into Controller to trigger background tasks.
            //Queue.QueueBackgroundWorkItem(async token => {});
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2
            AddHostedServices(services);

        }
        #endregion

        #region Notification Services
        public virtual void ConfigureNotificationServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Notification Services");

            services.AddSingleton<INotificationService, CompositeNotificationService>();
        }
        #endregion

        //https://github.com/RickStrahl/Westwind.AspnetCore.LiveReload
        #region Live Reload
        public virtual void ConfigureLiveReloadServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Live Reload Services");

            services.AddLiveReload();
        }
        #endregion

        public abstract void AddDatabases(IServiceCollection services, ConnectionStrings connectionStrings, string tenantsConnectionString, string identityConnectionString, string hangfireConnectionString, string defaultConnectionString);
        public abstract void AddUnitOfWorks(IServiceCollection services);
        public abstract void AddHostedServices(IServiceCollection services);
        public abstract void AddHangfireJobServices(IServiceCollection services);
        public virtual void AddGraphQLSchemas(IApplicationBuilder app)
        {

        }
        #endregion

        #region 2. Configure Autofac Container
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Logger.LogInformation("Configuring Autofac Modules");

            builder.RegisterModule(new AutofacConventionsDependencyInjectionModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacTasksModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacConventionsSignalRHubModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacAutomapperModule() { Filter = AssemblyBoolFilter });

            builder.RegisterTaskRunners();
        }
        #endregion

        #region 3. Configure Request Pipeline
        private static bool IsStreamRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var stream = false;

            var filename = Path.GetFileName(context.Request.Path.ToString());


            return stream;
        }

        private static bool AreCookiesConsentedCallback(Microsoft.AspNetCore.Http.HttpContext context, string cookieConsentName)
        {
            return context.Request.IsApi() || (context.Request.Cookies.Keys.Contains(cookieConsentName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //In older tutorials, you may see similar code in the Configure method in Startup.cs. We recommend that you use the Configure method only to set up the request pipeline. Application startup code belongs in the Main method.
        //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-2.2
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, AppSettings appSettings, CacheSettings cacheSettings, AuthorizationSettings authorizationSettings,
            LocalizationSettings localizationSettings, SwitchSettings switchSettings, ServerSettings serverSettings, TaskRunnerAfterApplicationConfiguration taskRunner, RequestLocalizationOptions localizationOptions,
            ISignalRHubMapper signalRHubMapper, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            Logger.LogInformation("Configuring Request Pipeline");

            //dotnet watch run
            // Before any other output generating middleware handlers
            if(HostingEnvironment.IsDevelopment())
            {
                app.UseLiveReload();
            }

            //https://github.com/aspnet/AspNetCore/blob/c565386a3ed135560bc2e9017aa54a950b4e35dd/src/Mvc/Mvc.Core/src/Routing/UrlHelperFactory.cs
            //There seems to be a big differences between UrlHelper and EndpointRoutingUrlHelper

            //--------------------------------------------- ROUTING -----------------------------//
            //.NET Core 3.0
            // 1. Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            //app.UseClientSideBlazorFiles<Client.Startup>();
            //app.UseRouting();
            //app.UseEndpointRouting();

            if (HostingEnvironment.IsProduction())
            {
                app.UseRobotsTxt(builder =>
                builder
                .AddSection(section =>
                   section
                       .AddComment("Allow All")
                       .AddUserAgent("*")
                       .Allow("/")
                   )
                   .AddSitemap($"http://{appSettings.SiteUrl}/sitemap.xml")
               );
            }
            else
            {
                app.UseRobotsTxt(builder =>
                builder
                .AddSection(section =>
                  section
                      .AddComment("Disallow All")
                      .AddUserAgent("*")
                      .Disallow("/")
                  ));
            }

            app.UseHealthChecks("/hc");

            if (!env.IsProduction())
            {
                app.UsePing("/ping");

                //Imortant: Must be before exception handling
                //1. download profiler from https://stackify.com/prefix/
                //2. enable .NET profiler in windows tray
                //3. access results at http: //localhost:2012
                app.UseStackifyPrefix();

                app.UseOutbound(appBranch =>
                {
                    appBranch.UseWhen(context => context.Request.IsMvc(), mvcBranch => mvcBranch.UseDeveloperExceptionPage());
                    appBranch.UseWhen(context => context.Request.IsApi(), apiBranch =>
                    {
                        apiBranch.UseProblemDetailsExceptionHandler(options => options.ShowExceptionDetails = true);
                        apiBranch.UseProblemDetailsErrorResponseHandler(options => options.HandleContentResponses = false);
                    });
                });

                app.UseDatabaseErrorPage();

                app.UseBrowserLink();

            }
            else
            {
                app.UseOutbound(appBranch =>
                {
                    appBranch.UseWhen(context => context.Request.IsMvc(), mvcBranch => mvcBranch.UseExceptionHandler("/Error"));
                    appBranch.UseWhen(context => context.Request.IsApi(), apiBranch =>
                    {
                        apiBranch.UseProblemDetailsExceptionHandler(options => options.ShowExceptionDetails = false);
                        apiBranch.UseProblemDetailsErrorResponseHandler(options => options.HandleContentResponses = false);
                    });
                });

                if (switchSettings.EnableHsts)
                {
                    //Only ever use HSTS in production!!!!!
                    //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                    app.UseHsts();
                }
            }

            if (switchSettings.EnableRedirectHttpToHttps)
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                //picks up port automatically
                app.UseHttpsRedirection();
            }

            if (switchSettings.EnableRedirectNonWwwToWww)
            {
                var options = new RewriteOptions();
                options.AddRedirectToWww();
                //options.AddRedirectToHttps(StatusCodes.Status307TemporaryRedirect); // Does not pick up port automatically
                app.UseRewriter(options);
            }

            if (switchSettings.EnableHelloWorld)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }

            app.UseRequestTasks();

            if (switchSettings.EnableSwagger)
            {
                var swaggerPrefix = "api";

                var swaggerEndpoints = new Dictionary<string, string>();
                foreach (var apiDescription in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    var swaggerEndpoint = $"/swagger/{apiDescription.GroupName}/swagger.json";
                    swaggerEndpoints.Add(apiDescription.GroupName, swaggerEndpoint);
                }

                if (!string.IsNullOrWhiteSpace(appSettings.SwaggerUIUsername) && !string.IsNullOrWhiteSpace(appSettings.SwaggerUIPassword))
                {
                    var swaggerSecured = swaggerEndpoints.Values.ToList().Concat(new[]{
                        $"/{swaggerPrefix}",
                        $"/{swaggerPrefix}/index.html" });

                    app.UseWhen(context => swaggerSecured.Contains(context.Request.Path.ToString()),
                     appBranch =>
                     {
                         appBranch.UseBasicAuth(appSettings.SwaggerUIUsername, appSettings.SwaggerUIPassword);
                     });
                }

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    //c.InjectStylesheet("/Assets/custum-ui.css");
                    //c.IndexStream = () => GetType().Assembly.GetManifestResourceStream(".html");

                    foreach (var swaggerEndpoint in swaggerEndpoints)
                    {
                        c.SwaggerEndpoint(swaggerEndpoint.Value, appSettings.AssemblyPrefix + " API " + swaggerEndpoint.Key.ToUpperInvariant());
                    }

                    c.RoutePrefix = swaggerPrefix;
                    c.DocExpansion(DocExpansion.None);
                    c.DefaultModelRendering(ModelRendering.Example);
                    c.EnableDeepLinking();
                    c.DisplayOperationId();
                });
            }

            //Use Response Compression Middleware when you're:
            //Unable to use the following server-based compression technologies:
            //IIS Dynamic Compression module
            //http://www.talkingdotnet.com/how-to-enable-gzip-compression-in-asp-net-core/
            // General
            //"text/plain",
            // Static files
            //"text/css",
            //"application/javascript",
            // MVC
            //"text/html",
            //"application/xml",
            //"text/xml",
            //"application/json",
            //"text/json",
            if (switchSettings.EnableResponseCompression)
            {
                //https://www.softfluent.com/blog/dev/Enabling-gzip-compression-with-ASP-NET-Core
                //Concerning performance, the middleware is about 28% slower than the IIS compression (source). Additionally, IIS or nginx has a threshold for compression to avoid compressing very small files.
                app.UseResponseCompression();
            }

            //API rate limiting
            if (switchSettings.EnableIpRateLimiting)
            {
                app.UseIpRateLimiting();
            }

            if (switchSettings.EnableSignalR)
            {
                //.NET Core 3.0 - move to endpoint routing
                app.UseSignalR(routes =>
                {
                    routes.MapHub<NotificationHub>(appSettings.SignalRUrlPrefix + "/signalr/notifications");
                    signalRHubMapper.MapHubs(routes, appSettings.SignalRUrlPrefix);
                });
            }

            //Cache-Control:max-age=0
            //This is equivalent to clicking Refresh, which means, give me the latest copy unless I already have the latest copy.
            //Cache-Control:no-cache 
            //This is holding Shift while clicking Refresh, which means, just redo everything no matter what.

            //Should only be used for server side HTML cachcing or Read Only API. Doesn't really make sense to use Response Caching for CRUD API. 

            //Will only attempt serve AND store caching if:
            //1. Controller or Action has ResponseCache attribute with Location = ResponseCacheLocation.Any
            //2. Request method is GET OR HEAD
            //3. AND Authorization header is not included

            //Will only attempt to serve from cache if:
            //1. Request header DOES NOT contain Cache-Control: no-cache (HTTP/1.1) AND Pragma: no-cache (HTTP/1.0)
            //2. AND Request header DOES NOT contain Cache-Control: max-age=0. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            //3. AND Request header If-None-Match != Cached ETag
            //4. AND Request header If-Modified-Since < Cached Last Modified (Time it was stored in cache)

            //Will only attempt to store in cache if:
            //1. Request header DOES NOT contain Cache-Control: no-store
            //2. AND Response header DOES NOT contain Cache-Control: no-store
            //3. AND Response header does not contain Set-Cookie
            //4. AND Response Status is 200

            //When storing
            //1. Stores all headers except Age
            //2. Stores Body
            //3. Stores Length

            //In memory cache
            //https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core
            //Unfortunately, the built-in response caching middleware makes this very difficult. 
            //Firstly, the same cache duration is used for both client and server caches. Secondly, currently there is no easy way to invalidate cache entries.
            //app.UseResponseCaching();
            //Request Header Cache-Control: max-age=0 or no-cache will bypass Response Caching. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            if (switchSettings.EnableResponseCaching)
            {
                if (switchSettings.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseResponseCachingCustom(); //Allows Invalidation
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                       appBranch =>
                       {
                           appBranch.UseResponseCachingCustom(); //Allows Invalidation
                       }
                     );
                }
            }

            //Works for: GET, HEAD (efficiency, and saves bandwidth)
            //Works for: PUT, PATCH (Concurrency)s
            //This is Etags
            //Generating ETags is expensive. Putting this after response caching makes sense.
            if (switchSettings.EnableETags)
            {
                if (switchSettings.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseHttpCacheHeaders();
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                     appBranch =>
                     {
                         appBranch.UseHttpCacheHeaders();
                     }
                   );
                }
            }

            app.MapWhen(
               context => context.Request.Path.ToString().StartsWith(AssetsFolder),
               appBranch =>
               {
                   // ... optionally add more middleware to this branch
                   char[] seperator = { ',' };
                   List<string> publicUploadFolders = appSettings.PublicUploadFolders.Split(seperator).ToList();
                   appBranch.UseContentHandler(env, AppSettings, publicUploadFolders, cacheSettings.UploadFilesDays);
               });

            app.UseDefaultFiles();

            //versioned files can have large cache expiry time
            app.UseVersionedStaticFiles(cacheSettings.VersionedStaticFilesDays);

            //non versioned files
            app.UseNonVersionedStaticFiles(cacheSettings.NonVersionedStaticFilesDays);

            if (switchSettings.EnableCookieConsent)
            {
                app.UseCookiePolicy();
            }

            app.UseJwtCookieAuthentication();

            //--------------------------------------------- ROUTING -----------------------------//
            //.NET Core 3.0
            // 1. Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            //app.UseClientSideBlazorFiles<Client.Startup>();
            //app.UseRouting();

            // 2. Middleware that run after routing occurs.
            app.UseAuthentication();

            //.NET Core 3.0
            var globalPolicyBuilder = new AuthorizationPolicyBuilder();
            if (authorizationSettings.UserMustBeAuthorizedByDefault)
            {
                globalPolicyBuilder.RequireAuthenticatedUser();
            }
            //var globalPolicy = globalPolicyBuilder.Build();
            //app.UseAuthorization(globalPolicy);

            if (switchSettings.EnableCors)
            {
                if (HostingEnvironment.IsProduction())
                {
                    app.UseCors("AllowSpecificOrigin");
                }
                else
                {
                    app.UseCors("AllowAnyOrigin");
                }
            }

            if (switchSettings.EnableHangfire)
            {
                if (switchSettings.EnableMultiTenancy)
                {
                    app.UseHangfireDashboardMultiTenant();
                }
                else
                {
                    app.UseHangfire(serverSettings.ServerName);
                }
            }

            app.UseWebSockets();

            AddGraphQLSchemas(app);
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            app.UseRequestLocalization(localizationOptions);

            app.UseMiniProfiler();

            var routeBuilder = new RouteBuilder(app);

            app.UseRouter(routeBuilder.Build());

            app.UseMvc(routes =>
            {
                routes.MapAllRoutes("/all-routes");

                //https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-2.2
                if (localizationSettings.AlwaysIncludeCultureInUrl)
                {
                    routes.RedirectCulturelessToDefaultCulture(localizationOptions);
                }
            });

            //.NET Core 3.0
            //https://devblogs.microsoft.com/aspnet/asp-net-core-updates-in-net-core-3-0-preview-4/
            // 3. Executes the endpoint that was selected by routing. replaces UseMvc and UseRouter
            //app.UseEndpoints(endpoints =>
            //{
            //    // Mapping of endpoints goes here:
            //    endpoints.MapHealthChecks("/hc");
            //    endpoints.MapAllRoutes("/all-routes");
            //    endpoints.MapControllers();
            //    endpoints.MapRazorPages();
            //    endpoints.MapBlazorHub();

            //    endpoints.MapHub<NotificationHub>(appSettings.SignalRUrlPrefix + "/signalr/notifications");
            //    endpoints.MapGrpcService<MyCalculatorService>();
            //    endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
            //});

            // 4. Middleware here will only run if nothing was matched.
            taskRunner.RunTasksAfterApplicationConfiguration();
        }
        #endregion
    }
}
