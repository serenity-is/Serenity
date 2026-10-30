namespace Serenity.Testing;

/// <summary>
///   User names used by the test helpers (login helpers, mock user registrations).
///   Applications with different user names can set these, either explicitly
///   (<see cref="Configure"/>) or through the "TestUsers" section of
///   testsettings.json / testsettings.machine.json, which the live app test
///   context (<see cref="FunctionalTestContext{TStartup}"/>) applies automatically.
/// </summary>
public static class TestUser
{
    private static string sysAdmin = "admin";
    private static string guest = "guest";

    /// <summary>User name used by <c>LoginAsSysAdminAsync</c> / <c>UseSysAdmin</c>.</summary>
    public static string SysAdmin
    {
        get => sysAdmin;
        set => sysAdmin = string.IsNullOrEmpty(value) ? "admin" : value;
    }

    /// <summary>User name used by <c>UseGuest</c>.</summary>
    public static string Guest
    {
        get => guest;
        set => guest = string.IsNullOrEmpty(value) ? "guest" : value;
    }

    /// <summary>Overrides the defaults for the current test run.</summary>
    public static void Configure(string sysAdmin = null, string guest = null)
    {
        if (!string.IsNullOrEmpty(sysAdmin))
            TestUser.sysAdmin = sysAdmin;

        if (!string.IsNullOrEmpty(guest))
            TestUser.guest = guest;
    }
}
