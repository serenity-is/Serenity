namespace Serenity.Web;

public class DynamicScriptTests
{
    public class TestData
    {
        public int Value { get; set; }
    }

    [DataScript("TestKey", CacheDuration = 60, Permission = "P", CacheGroupKey = "G")]
    private class KeyedDataScript : DataScript<TestData>
    {
        protected override TestData GetData() => new() { Value = 42 };
    }

    [DataScript]
    private class AutoKeyDataScript : DataScript<TestData>
    {
        protected override TestData GetData() => new() { Value = 1 };
    }

    private class TestDynamicScript : DynamicScript
    {
        public override string GetScript() => "script";
    }

    [Fact]
    public void DataScript_Throws_ForNullGetData()
    {
        Assert.Throws<ArgumentNullException>(() => new DataScript("key", null!));
    }

    [Fact]
    public void DataScript_ReturnsData()
    {
        var script = new DataScript("key", () => "data");
        Assert.Equal("RemoteData.key", script.ScriptName);
        Assert.Equal("data", script.GetScriptData());
        Assert.Contains("RemoteData.key", script.GetScript(), StringComparison.Ordinal);
    }

    [Fact]
    public void DataScriptOfT_UsesAttributeSettings()
    {
        var script = new KeyedDataScript();

        Assert.Equal("RemoteData.TestKey", script.ScriptName);
        Assert.Equal(TimeSpan.FromSeconds(60), script.Expiration);
        Assert.Equal("P", script.Permission);
        Assert.Equal("G", script.GroupKey);
        var data = Assert.IsType<TestData>(script.GetScriptData());
        Assert.Equal(42, data.Value);
        Assert.Contains("42", script.GetScript(), StringComparison.Ordinal);
    }

    [Fact]
    public void DataScriptOfT_AutoGeneratesKey()
    {
        var script = new AutoKeyDataScript();
        Assert.Contains("AutoKeyDataScript", script.ScriptName, StringComparison.Ordinal);
    }

    [Fact]
    public void DynamicScript_CheckRights_Throws_WhenPermissionDenied()
    {
        var script = new TestDynamicScript { Permission = "P" };
        Assert.Throws<ValidationError>(() =>
            script.CheckRights(new MockPermissions(_ => false), NullTextLocalizer.Instance));
    }

    [Fact]
    public void DynamicScript_CheckRights_Passes_WhenPermissionGranted()
    {
        var script = new TestDynamicScript { Permission = "P" };
        script.CheckRights(new MockPermissions(_ => true), NullTextLocalizer.Instance);
    }

    [Fact]
    public void DynamicScript_CheckRights_DoesNothing_WithoutPermission()
    {
        var script = new TestDynamicScript();
        script.CheckRights(new MockPermissions(_ => false), NullTextLocalizer.Instance);
    }

    [Fact]
    public void ConcatenatedScript_Throws_ForNullParts()
    {
        Assert.Throws<ArgumentNullException>(() => new ConcatenatedScript(null!));
    }

    [Fact]
    public void ConcatenatedScript_ConcatenatesParts()
    {
        var script = new ConcatenatedScript([() => "first", () => "second"]);
        var result = script.GetScript();
        Assert.Contains("first", result, StringComparison.Ordinal);
        Assert.Contains("second", result, StringComparison.Ordinal);
        Assert.Contains(";", result, StringComparison.Ordinal);
    }

    [Fact]
    public void ConcatenatedScript_CheckRights_InvokesCallback()
    {
        var invoked = false;
        var script = new ConcatenatedScript([() => "x"], checkRights: (_, _) => invoked = true);
        script.CheckRights(new MockPermissions(_ => true), NullTextLocalizer.Instance);
        Assert.True(invoked);
    }

    [Fact]
    public void ConcatenatedScript_WithEmptySeparator()
    {
        var script = new ConcatenatedScript([() => "a", () => "b"], separator: "");
        var result = script.GetScript();
        Assert.Contains("a", result, StringComparison.Ordinal);
        Assert.Contains("b", result, StringComparison.Ordinal);
    }
}
