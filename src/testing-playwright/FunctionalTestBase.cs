using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Serenity.Navigation;
using System.Diagnostics;
using System.IO;
using Xunit.Sdk;

namespace Serenity.Testing.Playwright;

/// <summary>
///   Base class for Playwright page tests of a Serenity application. It connects
///   the browser (<see cref="PageTest"/>) to the live in-process test application
///   (<see cref="ISerenityTestApp"/>), restores the in-memory database before
///   each test, logs to the test output and captures a screenshot / console output
///   when a test fails.
/// </summary>
public abstract class FunctionalTestBase : PageTest, IFunctionalTestContext
{
    protected readonly ISerenityTestApp app;
    private readonly ITestOutputHelper console;
    private Exception testException;
    private string testClassName;
    private string testName;
    private string testFullName;

    static FunctionalTestBase()
    {
        PlaywrightSettings.Initialize();
    }

    protected FunctionalTestBase(ISerenityTestApp app, ITestOutputHelper console)
    {
        this.console = console;
        GetTestInfo();
        Log($"Starting {testFullName}");
        this.app = app;
        app.Database.RestoreSnapshots();

        RegisterExceptionHook();
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions()
        {
            ViewportSize = new()
            {
                Width = 1920,
                Height = 1080
            },
            IgnoreHTTPSErrors = true
        };
    }

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync().ConfigureAwait(false);
        await Context.ClearCookiesAsync().ConfigureAwait(false);
    }

    private void GetTestInfo()
    {
        testClassName = GetType().Name;
        testFullName = testClassName;

        if (console == null)
            return;

        if (TestContext.Current is not TestContext test)
            return;

        testClassName = test.TestCase.TestMethod.TestClass.TestClassName;
        testName = test.TestCase.TestMethod.MethodName;
        testFullName = testClassName + "." + testName;
    }

    private static bool IsFunctionalTestType(Type type)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            if (current == typeof(FunctionalTestBase))
                return true;
        }

        return false;
    }

    private void RegisterExceptionHook()
    {
        AppDomain.CurrentDomain.FirstChanceException += (_, e) =>
        {
            var exception = e.Exception;

            if (exception == null)
                return;

            if (exception is XunitException)
            {
                testException = exception;
                return;
            }

            if (exception is TargetInvocationException targetInvocationException)
                exception = targetInvocationException.InnerException;

            StackTrace outerTrace = new(exception, false);
            var firstFrame = outerTrace.GetFrame(outerTrace.FrameCount - 1);
            var firstMethod = firstFrame?.GetMethod();

            if (firstMethod?.DeclaringType == null ||
                !IsFunctionalTestType(firstMethod.DeclaringType))
                return;

            testException = exception;
        };
    }

    public Uri AppUri => app.AppUri;

    public MemoryDbBase Database => app.Database;

    /// <summary>User names used by the login helpers (from testsettings).</summary>
    protected TestUserSettings TestUsers => app.TestUsers;

    public IDbConnection NewConnectionFor(RowFieldsBase fld)
    {
        return app.Services.GetRequiredService<ISqlConnections>().NewByKey(fld.ConnectionKey);
    }

    public IDbConnection NewDefaultConnection()
    {
        return app.Services.GetRequiredService<ISqlConnections>().NewByKey(DefaultConnectionAttribute.Key);
    }

    public async Task GoToUrlAsAsync(string relativeUrl, string username)
    {
        await this.LoginAsAsync(username);
        await Page.GotoAsync(new Uri(AppUri, relativeUrl).ToString());
    }

    public async Task GoToUrlAsSysAdminAsync(string relativeUrl)
    {
        await this.LoginAsSysAdminAsync();
        await Page.GotoAsync(new Uri(AppUri, relativeUrl).ToString());
    }

    public async Task GoToUrlAsync(string relativeUrl)
    {
        await Page.GotoAsync(new Uri(AppUri, relativeUrl).ToString());
    }

    public async ValueTask<TPageObject> FindAsync<TPageObject>(bool topmost = false)
        where TPageObject : PageObject, new()
    {
        return await this.FindPageObjectAsync<TPageObject>(topmost);
    }

    public async Task WaitForAjaxAsync(int seconds = 30)
    {
        await Page.WaitForAjaxAsync(seconds);
    }

    public async Task ResetDynamicScriptsAsync(bool reloadPage = true)
    {
        app.Services.GetService<IDynamicScriptManager>().Reset();

        if (reloadPage && !string.IsNullOrEmpty(Page.Url))
            await Page.GotoAsync(Page.Url.ToString());
    }

    public static string GetUrlFor<TController>(string action = "Index")
        where TController : ControllerBase
    {
        var url = NavigationLinkAttribute.GetUrlFromController(typeof(TController), action);
        if (url.StartsWith("~/"))
            url = url[2..];
        return url;
    }

    public void CreateFunction<T1, TResult>(IDbConnection connection, string name, Func<T1, TResult> function, bool isDeterministic = false)
    {
        var con = (SqliteConnection)((IHasActualConnection)connection).ActualConnection;
        con.CreateFunction(name, function, isDeterministic);
        OnConnectionOpen += cnn => cnn.CreateFunction(name, function, isDeterministic);
    }

    public void CreateFunction<T1, T2, TResult>(IDbConnection connection, string name, Func<T1, T2, TResult> function, bool isDeterministic = false)
    {
        var con = (SqliteConnection)((IHasActualConnection)connection).ActualConnection;
        con.CreateFunction(name, function, isDeterministic);
        OnConnectionOpen += cnn => cnn.CreateFunction(name, function, isDeterministic);
    }

    public Action<SqliteConnection> OnConnectionOpen;

    public void Log(string message)
    {
        console?.WriteLine(message);
    }

    public async override ValueTask DisposeAsync()
    {
        if (!TestOk())
        {
            try
            {
                foreach (var message in await Page.ConsoleMessagesAsync())
                {
                    if (message.Type == "error" ||
                        message.Type == "warning")
                    {
                        Log($"Page console {message.Type}: {message.Text}");
                        Log(message.Text);
                    }
                }

                var screenshot = await Page.ScreenshotAsync();
                var buildArtifactsPath = Environment.GetEnvironmentVariable("ARTIFACT_DIRECTORY") ??
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "artifacts");

                buildArtifactsPath = Path.Combine(buildArtifactsPath, "TestResults", "Screenshots", testClassName);

                if (!Directory.Exists(buildArtifactsPath))
                    Directory.CreateDirectory(buildArtifactsPath);

                var screenshotPath = Path.Combine(buildArtifactsPath, $"{DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}.{testFullName}.png");
                await File.WriteAllBytesAsync(screenshotPath, screenshot);
                Log($"Artifact saved to: {screenshotPath}");
            }
            catch
            {
                // capturing the failure artifact must not hide the real failure
            }
        }

        await base.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
