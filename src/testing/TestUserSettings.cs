namespace Serenity.Testing;

/// <summary>
///   User names used by the test helpers. Bound from the "TestUsers" section of
///   testsettings.json / testsettings.machine.json (both copied to the test
///   output folder), so a project can use its own user names without touching
///   test code.
/// </summary>
public class TestUserSettings
{
    public const string SectionKey = "TestUsers";

    public string SysAdmin { get; set; } = "admin";

    public string Guest { get; set; } = "guest";

    /// <summary>Applies these names to the <see cref="TestUser"/> defaults.</summary>
    public void Apply()
    {
        TestUser.Configure(SysAdmin, Guest);
    }
}
