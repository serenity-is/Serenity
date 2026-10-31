using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Serenity.Testing;

/// <summary>
///   Boots <typeparamref name="TStartup"/> in-process on a random HTTPS port
///   with an in-memory SQLite database, so Playwright (or other functional)
///   tests can drive the real application without any external service.
///   The derived class only has to tell where the web project lives and which
///   connection keys the application uses.
/// </summary>
/// <typeparam name="TStartup">The application's Startup class.</typeparam>
public abstract class FunctionalTestContext<TStartup> : IDisposable, ISerenityTestApp
    where TStartup : class
{
    private static readonly MethodInfo configureServicesMethod = GetStartupMethod("ConfigureServices", typeof(IServiceCollection))
        ?? throw new InvalidOperationException($"{typeof(TStartup).Name} has no ConfigureServices(IServiceCollection) method");

    // looked up with WebApplication argument type, so it matches both Configure(IApplicationBuilder, ...) and Configure(WebApplication, ...)
    private static readonly MethodInfo configureMethod = GetStartupMethod("Configure", typeof(WebApplication), typeof(IWebHostEnvironment))
        ?? throw new InvalidOperationException($"{typeof(TStartup).Name} has no Configure(IApplicationBuilder or WebApplication, IWebHostEnvironment) method");

    private bool disposed;
    private IServer server;
    private WebApplication app;
    private MemoryDbBase database;
    private TestUserSettings testUsers;
    private string webProjectPath;
    private Uri appUri;

    protected FunctionalTestContext()
    {
        // load testsettings.json / testsettings.machine.json (TestUsers) eagerly,
        // so the TestUser defaults are applied before any test runs
        _ = TestUsers;
    }

    private static MethodInfo GetStartupMethod(string name, params Type[] parameters) =>
        typeof(TStartup).GetMethod(name, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);

    /// <summary>
    ///   Absolute path of the web project folder (the folder with appsettings.json
    ///   and wwwroot). Override it, or set the SERENITY_TEST_WEB_PROJECT
    ///   environment variable (useful on CI).
    /// </summary>
    protected virtual string ResolveWebProjectPath()
    {
        var overridden = Environment.GetEnvironmentVariable("SERENITY_TEST_WEB_PROJECT");
        if (!string.IsNullOrWhiteSpace(overridden))
            return Path.GetFullPath(overridden);

        throw new InvalidOperationException(
            $"{GetType().Name} must override ResolveWebProjectPath() or set the " +
            "SERENITY_TEST_WEB_PROJECT environment variable.");
    }

    /// <summary>Creates the in-memory database with the application's connection keys.</summary>
    protected abstract MemoryDbBase CreateDatabase();

    /// <summary>
    ///   Configuration applied on top of the web project's appsettings.json, e.g.
    ///   to disable background jobs. Connection strings are added automatically;
    ///   null values remove a key.
    /// </summary>
    protected virtual IDictionary<string, string> TestSettings { get; } = new Dictionary<string, string>
    {
        ["BackgroundJobs:Enabled"] = "False",
        ["StartNodeScripts"] = null
    };

    /// <summary>Adds the web project's own configuration files.</summary>
    protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configuration)
    {
        configuration.AddJsonFile("appsettings.bundles.json", optional: true);
        configuration.AddJsonFile("appsettings.machine.json", optional: true);
    }

    /// <summary>
    ///   Extra middleware/endpoints inserted before the application's own pipeline.
    ///   Override and call the base to keep the test login/callback endpoints.
    /// </summary>
    public virtual void ConfigureTestPipeline(IApplicationBuilder app)
    {
        // not using app.Map here, as it moves "/test-session-login" to Request.PathBase, and as this runs
        // before UseAuthentication, the cookie handler would issue the cookie for that path only
        app.Use(async (httpContext, next) =>
        {
            if (!httpContext.Request.Path.StartsWithSegments("/test-session-login", out var remaining) ||
                !remaining.HasValue)
            {
                await next();
                return;
            }

            var username = remaining.ToString()[1..];
            var principal = httpContext.RequestServices.GetRequiredService<IUserClaimCreator>()
                .CreatePrincipal(username, authType: "Test");
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            await httpContext.Response.WriteAsync(remaining);
        });

        app.Map("/test/callback", branch =>
        {
            branch.Run(async httpContext =>
            {
                OpenIdTestCallback?.Invoke(null, httpContext.Request.QueryString.ToString());
                await httpContext.Response.WriteAsync("OK");
            });
        });
    }

    /// <summary>Raised when the test callback endpoint is hit (OpenId tests).</summary>
    public EventHandler<string> OpenIdTestCallback { get; set; }

    /// <summary>Services of the running application.</summary>
    public virtual IServiceProvider Services
    {
        get
        {
            EnsureServer();
            return app?.Services;
        }
    }

    /// <summary>The running server.</summary>
    public IServer Server
    {
        get
        {
            EnsureServer();
            return server;
        }
    }

    /// <summary>Base uri (random HTTPS port) of the running application.</summary>
    public Uri AppUri
    {
        get
        {
            EnsureServer();
            return appUri;
        }
    }

    /// <summary>Absolute path of the web project (content root).</summary>
    public virtual string WebProjectPath => webProjectPath ??= ResolveWebProjectPath();

    /// <summary>In-memory database of the application.</summary>
    public MemoryDbBase Database
    {
        get
        {
            EnsureDatabase();
            return database;
        }
    }

    /// <summary>User names used for logins; applied from testsettings files.</summary>
    public TestUserSettings TestUsers
    {
        get
        {
            if (testUsers != null)
                return testUsers;

            testUsers = LoadTestUsers();
            testUsers.Apply();
            return testUsers;
        }
    }

    private static TestUserSettings LoadTestUsers()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "testsettings.json"), optional: true)
            .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "testsettings.machine.json"), optional: true)
            .Build();

        return configuration.GetSection(TestUserSettings.SectionKey).Get<TestUserSettings>()
            ?? new TestUserSettings();
    }

    /// <summary>Absolute path of the test assembly.</summary>
    protected virtual string GetTestDllPath()
    {
        var location = GetType().Assembly.Location;
        if (!string.IsNullOrEmpty(location))
            return new Uri(location).LocalPath;

        return Environment.CurrentDirectory;
    }

    /// <summary>Folder the test assembly is built into (e.g. artifacts/bin/Project/config).</summary>
    protected virtual string TestProjectRoot => Path.GetDirectoryName(
        Path.GetDirectoryName(Path.GetDirectoryName(GetTestDllPath())));

    private void EnsureDatabase()
    {
        if (database != null)
            return;

        database = CreateDatabase() ?? throw new InvalidOperationException($"{GetType().Name}.CreateDatabase() returned null");
    }

    /// <summary>Recreates the in-memory database (drops all data).</summary>
    public void ResetDb()
    {
        if (database == null)
            return;

        database.Dispose();
        database = CreateDatabase();
    }

    private void EnsureServer()
    {
        if (server != null)
            return;

        EnsureDepsAndStaticAssetsFiles();
        EnsureDatabase();
        _ = TestUsers; // applies the configured user names before the app starts

        var builder = CreateWebApplicationBuilder();

        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
            socket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            socket.Listen(1);
            var endPoint = (IPEndPoint)socket.LocalEndPoint;

            builder.WebHost.ConfigureKestrel(k =>
            {
                k.Listen(endPoint, o => o.UseHttps());
            });

            appUri = new Uri("https://" + endPoint);
        }

        var startup = Activator.CreateInstance(typeof(TStartup), builder.Configuration, builder.Environment);
        configureServicesMethod.Invoke(startup, [builder.Services]);
        ConfigureTestServices(builder.Services);

        app = builder.Build();
        configureMethod.Invoke(startup, [app, app.Environment]);
        app.Start();

        server = app.Services.GetRequiredService<IServer>();

        Database.CreateSnapshots();
    }

    /// <summary>
    ///   Registers additional test services. The test pipeline filter is registered
    ///   here as well, so <see cref="ConfigureTestPipeline"/> runs before the
    ///   application's own pipeline.
    /// </summary>
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
        services.AddTransient<IStartupFilter>(_ => new TestPipelineStartupFilter(ConfigureTestPipeline));
    }

    protected virtual void EnsureDepsAndStaticAssetsFiles()
    {
        var assembly = typeof(TStartup).Assembly;
        if (assembly.EntryPoint == null)
            throw new InvalidOperationException($"Invalid Assembly Endpoint: {assembly.GetName().Name}");

        var baseName = Path.Combine(AppContext.BaseDirectory, assembly.GetName().Name!);

        var depsFile = baseName + ".deps.json";
        if (!File.Exists(depsFile))
            throw new InvalidOperationException($"Missing Deps File: {depsFile}");

        // apps with static web assets disabled only have the Serenity static assets manifest
        var staticAssetsFile = baseName + ".staticwebassets.runtime.json";
        var serenityStaticAssetsFile = baseName + SerenityStaticAssets.ManifestFileSuffix;
        if (!File.Exists(staticAssetsFile) && !File.Exists(serenityStaticAssetsFile))
            throw new InvalidOperationException($"Missing Static Assets File: {serenityStaticAssetsFile} or {staticAssetsFile}");
    }

    /// <summary>Creates the web application builder for the test application.</summary>
    protected virtual WebApplicationBuilder CreateWebApplicationBuilder()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = [],
            ApplicationName = typeof(TStartup).Assembly.GetName().Name,
            EnvironmentName = Environments.Development,
            ContentRootPath = WebProjectPath
        });

        builder.WebHost.UseStaticWebAssets();
        builder.WebHost.UseSerenityStaticAssets();
        ConfigureAppConfiguration(builder.Configuration);

        DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", Microsoft.Data.Sqlite.SqliteFactory.Instance);

        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in TestSettings ?? new Dictionary<string, string>())
            settings[pair.Key] = pair.Value;

        foreach (var connection in database.ListConnectionStrings())
        {
            settings["Data:" + connection.ConnectionKey + ":ConnectionString"] = connection.ConnectionString;
            settings["Data:" + connection.ConnectionKey + ":ProviderName"] = "Microsoft.Data.Sqlite";
        }

        builder.Configuration.AddInMemoryCollection(settings);

        return builder;
    }

    private sealed class TestPipelineStartupFilter(Action<IApplicationBuilder> configurePipeline) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            configurePipeline(app);
            next(app);
        };
    }

    ~FunctionalTestContext()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            database?.Dispose();
            server?.Dispose();
            (app as IHost)?.Dispose();
        }

        disposed = true;
    }
}
