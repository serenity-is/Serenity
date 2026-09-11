namespace Serenity.Web;

public class DynamicScriptManagerTests
{
    private class TestScript : DynamicScript, IGetScriptData
    {
        public int GetScriptCalls { get; private set; }

        public override string GetScript()
        {
            GetScriptCalls++;
            return "TEST";
        }

        public object GetScriptData() => new { A = 1 };
    }

    private class NoDataScript : DynamicScript
    {
        public override string GetScript() => "NODATA";
    }

    private class NamedScript : DynamicScript, INamedDynamicScript
    {
        public string ScriptName => "Named.Test";
        public override string GetScript() => "NAMED";
    }

    private class CacheSuffixScript : TestScript, ICacheSuffix
    {
        public string CacheSuffix => "v1";
    }

    private static DynamicScriptManager Create(IPermissionService? permissions = null)
    {
        return new DynamicScriptManager(new TestTwoLevelCache(),
            permissions ?? new MockPermissions(), NullTextLocalizer.Instance);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var cache = new TestTwoLevelCache();

        Assert.Throws<ArgumentNullException>(() =>
            new DynamicScriptManager(null!, new MockPermissions(), NullTextLocalizer.Instance));
        Assert.Throws<ArgumentNullException>(() =>
            new DynamicScriptManager(cache, null!, NullTextLocalizer.Instance));
    }

    [Fact]
    public void RegisteredScripts_Is_Registered_By_Default()
    {
        var manager = Create();

        Assert.True(manager.IsRegistered("RegisteredScripts"));
        Assert.Contains("RegisteredScripts", manager.GetRegisteredScriptNames());
        Assert.DoesNotContain("RegisteredScripts", manager.GetRegisteredScripts().Keys);
    }

    [Fact]
    public void Register_Throws_For_Null_Arguments()
    {
        var manager = Create();
        Assert.Throws<ArgumentNullException>(() => manager.Register((string)null!, new TestScript()));
        Assert.Throws<ArgumentNullException>(() => manager.Register("x", null!));
        Assert.Throws<ArgumentNullException>(() => manager.Register((INamedDynamicScript)null!));
    }

    [Fact]
    public void Register_And_GetScriptText_And_Include()
    {
        var manager = Create();
        manager.Register("Test", new TestScript());

        Assert.True(manager.IsRegistered("Test"));
        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        Assert.StartsWith("Test.js?v=", manager.GetScriptInclude("Test"));
        Assert.StartsWith("Test.css?v=", manager.GetScriptInclude("Test", ".css"));
    }

    [Fact]
    public void Register_Named_Script_Uses_ScriptName()
    {
        var manager = Create();
        manager.Register(new NamedScript());

        Assert.True(manager.IsRegistered("Named.Test"));
        Assert.Equal("NAMED", manager.GetScriptText("Named.Test", false));
    }

    [Fact]
    public void GetScriptText_Returns_Null_For_Unknown()
    {
        var manager = Create();
        Assert.Null(manager.GetScriptText("Unknown", false));
    }

    [Fact]
    public void GetScriptInclude_Returns_Name_For_Unknown()
    {
        var manager = Create();
        Assert.Equal("Unknown", manager.GetScriptInclude("Unknown"));
    }

    [Fact]
    public void GetScriptText_Json_Uses_Script_Data()
    {
        var manager = Create();
        manager.Register("Test", new TestScript());

        var json = manager.GetScriptText("Test", json: true);

        Assert.Contains("\"A\":1", json);
    }

    [Fact]
    public void GetScriptText_Json_Throws_When_Not_IGetScriptData()
    {
        var manager = Create();
        manager.Register("NoData", new NoDataScript());

        var exception = Assert.Throws<ValidationError>(() => manager.GetScriptText("NoData", json: true));
        Assert.Contains("IGetScriptData", exception.Message);
    }

    [Fact]
    public void Changed_Raises_Event_And_Invalidates_Cache()
    {
        var manager = Create();
        var script = new TestScript();
        manager.Register("Test", script);
        string? changedName = null;
        manager.ScriptChanged += name => changedName = name;

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        manager.Changed("Test");
        Assert.Equal("Test", changedName);

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        Assert.Equal(2, script.GetScriptCalls);
    }

    [Fact]
    public void Changed_Throws_For_Null()
    {
        var manager = Create();
        Assert.Throws<ArgumentNullException>(() => manager.Changed(null!));
    }

    [Fact]
    public void IfNotRegistered_Calls_Factory_Only_Once()
    {
        var manager = Create();
        int calls = 0;

        manager.IfNotRegistered("Test", () => { calls++; return new TestScript(); });
        manager.IfNotRegistered("Test", () => { calls++; return new TestScript(); });

        Assert.Equal(1, calls);
        Assert.True(manager.IsRegistered("Test"));
    }

    [Fact]
    public void Reset_Invalidates_Cached_Content()
    {
        var manager = Create();
        var script = new TestScript();
        manager.Register("Test", script);

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        manager.Reset();
        Assert.Equal("TEST", manager.GetScriptText("Test", false));

        Assert.Equal(2, script.GetScriptCalls);
    }

    [Fact]
    public void CheckScriptRights_Validates_Permission()
    {
        var manager = Create(new MockPermissions(_ => false));
        manager.Register("Perm", new TestScript { Permission = "X" });

        var exception = Assert.Throws<ValidationError>(() => manager.CheckScriptRights("Perm"));
        Assert.Equal("AccessDenied", exception.ErrorCode);
    }

    [Fact]
    public void CheckScriptRights_Passes_With_Permission()
    {
        var manager = Create(new MockPermissions(_ => true));
        manager.Register("Perm", new TestScript { Permission = "X" });

        manager.CheckScriptRights("Perm");
    }

    [Fact]
    public void CheckScriptRights_Does_Nothing_For_Unknown()
    {
        var manager = Create();
        manager.CheckScriptRights("Unknown");
    }

    [Fact]
    public void ReadScriptContent_Returns_Null_For_Unknown()
    {
        var manager = Create();
        Assert.Null(manager.ReadScriptContent("Unknown", false));
    }

    [Fact]
    public void ReadScriptContent_Checks_Rights_And_Returns_Content()
    {
        var manager = Create(new MockPermissions(_ => false));
        manager.Register("Perm", new TestScript { Permission = "X" });

        Assert.Throws<ValidationError>(() => manager.ReadScriptContent("Perm", false));
    }

    [Fact]
    public void ReadScriptContent_Returns_Content()
    {
        var manager = Create();
        manager.Register("Test", new TestScript());

        var content = manager.ReadScriptContent("Test", false);

        Assert.NotNull(content);
        Assert.Equal("TEST", System.Text.Encoding.UTF8.GetString(content!.Content));
    }

    [Fact]
    public void CacheSuffix_Affects_Include_Hash()
    {
        var manager = Create();
        manager.Register("Test", new CacheSuffixScript());

        var include = manager.GetScriptInclude("Test");

        Assert.StartsWith("Test.js?v=-", include);
    }

    [Fact]
    public void GroupKey_Uses_Local_Store()
    {
        var manager = Create();
        var script = new TestScript { GroupKey = "Group" };
        manager.Register("Test", script);

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        Assert.Equal(1, script.GetScriptCalls);
    }

    [Fact]
    public void ScriptChanged_Can_Be_Removed()
    {
        var manager = Create();
        int calls = 0;
        Action<string> handler = _ => calls++;

        manager.ScriptChanged += handler;
        manager.ScriptChanged -= handler;
        manager.Changed("Test");

        Assert.Equal(0, calls);
    }

    [Fact]
    public void PeekScriptHash_Throws_For_Null_Name()
    {
        var manager = Create();
        Assert.Throws<ArgumentNullException>(() => manager.PeekScriptHash(null!, new TestScript()));
    }

    [Fact]
    public void GetRegisteredScripts_Includes_Registered_Scripts()
    {
        var manager = Create();
        manager.Register("Test", new TestScript());

        var scripts = manager.GetRegisteredScripts();

        Assert.True(scripts.ContainsKey("Test"));
        Assert.False(scripts.ContainsKey("RegisteredScripts"));
    }

    [Fact]
    public void GetScriptInclude_Invalidates_Cached_Content_After_Change()
    {
        var manager = Create();
        var script = new TestScript();
        manager.Register("Test", script);

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        manager.Changed("Test");

        Assert.StartsWith("Test.js?v=", manager.GetScriptInclude("Test"));
    }

    [Fact]
    public void GroupKey_Content_Is_Invalidated_After_Change()
    {
        var manager = Create();
        var script = new TestScript { GroupKey = "Group" };
        manager.Register("Test", script);

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        manager.Changed("Test");

        Assert.Equal("TEST", manager.GetScriptText("Test", false));
        Assert.StartsWith("Test.js?v=", manager.GetScriptInclude("Test"));
    }
}
