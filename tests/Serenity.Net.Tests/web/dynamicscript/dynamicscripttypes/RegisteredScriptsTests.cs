namespace Serenity.Web;

public class RegisteredScriptsTests
{
    [Fact]
    public void Constructor_Throws_For_Null_Manager()
    {
        Assert.Throws<ArgumentNullException>(() => new RegisteredScripts(null!));
    }

    [Fact]
    public void Properties_Are_Set()
    {
        var manager = new MockDynamicScriptManager();
        var script = new RegisteredScripts(manager);

        Assert.Equal("RegisteredScripts", script.ScriptName);
        Assert.Equal(TimeSpan.FromDays(-1), script.Expiration);
    }

    [Fact]
    public void GetScriptData_Returns_Registered_Scripts()
    {
        var manager = new MockDynamicScriptManager
        {
            RegisteredScriptsResult = new Dictionary<string, string> { ["A"] = "h1" }
        };
        var script = new RegisteredScripts(manager);

        var data = Assert.IsAssignableFrom<IDictionary<string, string>>(script.GetScriptData());
        Assert.Equal("h1", data["A"]);
    }

    [Fact]
    public void GetScript_Includes_Registered_Hashes()
    {
        var manager = new MockDynamicScriptManager
        {
            RegisteredScriptsResult = new Dictionary<string, string> { ["A"] = "h1" }
        };
        var script = new RegisteredScripts(manager);

        var result = script.GetScript();

        Assert.Contains("setRegisteredScripts", result);
        Assert.Contains("\"A\":\"h1\"", result);
    }
}
