namespace Serenity.Testing;

/// <summary>
///   State shared by the live application test host and the Playwright test
///   bases: the running server's services, its base uri, the in-memory database
///   and the user names to use for logins.
/// </summary>
public interface ISerenityTestApp
{
    /// <summary>Services of the running test application.</summary>
    IServiceProvider Services { get; }

    /// <summary>Base uri of the test application (random HTTPS port).</summary>
    Uri AppUri { get; }

    /// <summary>In-memory database used by the test application.</summary>
    MemoryDbBase Database { get; }

    /// <summary>Absolute path of the web project (content root).</summary>
    string WebProjectPath { get; }

    /// <summary>User names used for logins in tests.</summary>
    TestUserSettings TestUsers { get; }

    /// <summary>Raised when the test callback endpoint (/test/callback) is hit.</summary>
    EventHandler<string> OpenIdTestCallback { get; set; }

    /// <summary>Recreates the in-memory database (drops all data).</summary>
    void ResetDb();
}
