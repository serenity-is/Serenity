using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serenity.Extensions.DependencyInjection;
using Serenity.Localization;
using System.IO;

namespace Serene;

public partial class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
    {
        Configuration = configuration;
        HostEnvironment = hostEnvironment;
        RegisterDataProviders();
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment HostEnvironment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationPartsFeatureToggles(Configuration);
        services.AddApplicationPartsTypeSource();
        services.ConfigureSections(Configuration);

        services.Configure<ForwardedHeadersOptions>(options => options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto);

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.SupportedUICultures = AppServices.UserCultureProvider.SupportedCultures;
            options.SupportedCultures = AppServices.UserCultureProvider.SupportedCultures;
            options.RequestCultureProviders.Insert(Math.Max(options.RequestCultureProviders.Count - 1, 0),
                new AppServices.UserCultureProvider()); // insert it before AcceptLanguage header provider
        });

        var dataProtectionKeysFolder = Configuration?["DataProtectionKeysFolder"];
        if (!string.IsNullOrEmpty(dataProtectionKeysFolder))
        {
            dataProtectionKeysFolder = Path.Combine(HostEnvironment.ContentRootPath, dataProtectionKeysFolder);
            if (Directory.Exists(dataProtectionKeysFolder))
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysFolder));
        }

        services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");
        services.Configure<KestrelServerOptions>(options => options.AllowSynchronousIO = true);
        services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);

        var builder = services.AddControllersWithViews(options =>
        {
            options.Filters.Add(typeof(AutoValidateAntiforgeryIgnoreBearerAttribute));
            options.Filters.Add(typeof(AntiforgeryCookieResultFilterAttribute));
            options.Conventions.Add(new ServiceEndpointActionModelConvention());
            options.ModelMetadataDetailsProviders.Add(new ServiceEndpointBindingMetadataProvider());
        });

        services.Configure<JsonOptions>(options => JSON.Defaults.Populate(options.JsonSerializerOptions));

        services.AddAuthentication(o =>
        {
            o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(o =>
        {
            o.Cookie.Name = ".AspNetAuth";
            o.LoginPath = new PathString("/Account/Login/");
            o.AccessDeniedPath = new PathString("/Account/AccessDenied");
            o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            o.SlidingExpiration = true;
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        });

        services.AddSingleton<IDataMigrations, AppServices.DataMigrations>();
        services.AddSingleton<IElevationHandler, DefaultElevationHandler>();
        services.AddSingleton<IEmailSender, EmailSender>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IHttpContextItemsAccessor, HttpContextItemsAccessor>();
        services.AddSingleton<INavigationModelFactory, AppServices.NavigationModelFactory>();
        services.AddSingleton<IPermissionService, AppServices.PermissionService>();
        services.AddSingleton<IPermissionKeyLister, AppServices.PermissionKeyLister>();
        services.AddSingleton<IRolePermissionService, AppServices.RolePermissionService>();
        services.AddSingleton<IUploadAVScanner, ClamAVUploadScanner>();
        services.AddSingleton<IUserPasswordValidator, AppServices.UserPasswordValidator>();
        services.AddUserProvider<AppServices.UserAccessor, AppServices.UserRetrieveService>();
        services.AddServiceHandlers();
        services.AddDynamicScripts();
        services.AddCssBundling();
        services.AddScriptBundling();
        services.AddUploadStorage();
        services.AddReporting();
    }

    public static void InitializeLocalTexts(IServiceProvider services)
    {
        var env = services.GetRequiredService<IWebHostEnvironment>();
        services.AddBaseTexts(env.WebRootFileProvider)
            .AddJsonTexts(env.WebRootFileProvider, "Scripts/site/texts")
            .AddJsonTexts(env.ContentRootFileProvider, "App_Data/texts");
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        RowFieldsProvider.SetDefaultFrom(app.ApplicationServices);

        var startNodeScripts = Configuration["StartNodeScripts"];
        if (!string.IsNullOrEmpty(startNodeScripts))
        {
            foreach (var script in startNodeScripts.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                app.StartNodeScript(script);
            }
        }

        InitializeLocalTexts(app.ApplicationServices);

        app.UseRequestLocalization();

        if (Configuration["UseForwardedHeaders"] == "True")
            app.UseForwardedHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        if (!string.IsNullOrEmpty(Configuration["UsePathBase"]))
            app.UsePathBase(Configuration["UsePathBase"]);

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        ConfigureTestPipeline?.Invoke(app);

        app.UseDynamicScripts();

        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });

        app.ApplicationServices.GetRequiredService<IDataMigrations>().Initialize();
    }

    public static Action<IApplicationBuilder> ConfigureTestPipeline { get; set; }

    public static void RegisterDataProviders()
    {
        DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
        DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);

        // to enable FIREBIRD: add FirebirdSql.Data.FirebirdClient reference, set connections, and uncomment line below
        // DbProviderFactories.RegisterFactory("FirebirdSql.Data.FirebirdClient", FirebirdSql.Data.FirebirdClient.FirebirdClientFactory.Instance);

        // to enable MYSQL: add MySqlConnector reference, set connections, and uncomment line below
        // DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlConnector.MySqlConnectorFactory.Instance);

        // to enable POSTGRES: add Npgsql reference, set connections, and uncomment line below
        // DbProviderFactories.RegisterFactory("Npgsql", Npgsql.NpgsqlFactory.Instance);
    }
}
