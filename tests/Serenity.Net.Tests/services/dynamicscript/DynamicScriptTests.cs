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
    public void DynamicScript_HasPermission_False_WhenPermissionDenied()
    {
        var script = new TestDynamicScript { Permission = "P" };
        Assert.False(script.HasPermission(new MockPermissions(_ => false)));
    }

    [Fact]
    public void DynamicScript_HasPermission_True_WhenPermissionGranted()
    {
        var script = new TestDynamicScript { Permission = "P" };
        Assert.True(script.HasPermission(new MockPermissions(x => x == "P")));
    }

    [Fact]
    public void DynamicScript_HasPermission_Denies_WithoutPermission()
    {
        var script = new TestDynamicScript();
        Assert.False(script.HasPermission(new MockPermissions(_ => true)));
    }

    [Fact]
    public void ConcatenatedScript_Throws_ForNullParts()
    {
        Assert.Throws<ArgumentNullException>(() => new ConcatenatedScript((IEnumerable<Func<string>>)null!));
        Assert.Throws<ArgumentNullException>(() => new ConcatenatedScript((IEnumerable<ConcatenatedScriptPart>)null!));
    }

    [Fact]
    public void ConcatenatedScript_Skips_Parts_Without_Permission()
    {
        var script = new ConcatenatedScript(
        [
            new ConcatenatedScriptPart(() => "first"),
            new ConcatenatedScriptPart(() => "denied", hasPermission: () => false),
            new ConcatenatedScriptPart(() => "granted", hasPermission: () => true)
        ]);

        var result = script.GetScript();
        Assert.Contains("first", result, StringComparison.Ordinal);
        Assert.Contains("granted", result, StringComparison.Ordinal);
        Assert.DoesNotContain("denied", result, StringComparison.Ordinal);
    }

    [Fact]
    public void ConcatenatedScript_CacheSuffix_Is_Empty_Without_Conditional_Parts()
    {
        var script = new ConcatenatedScript([() => "a", () => "b"]);
        Assert.Equal("", script.CacheSuffix);
    }

    [Fact]
    public void ConcatenatedScript_CacheSuffix_Varies_By_Accessible_Parts_And_Part_Suffixes()
    {
        var granted = true;
        var nestedSuffix = "x";
        var script = new ConcatenatedScript(
        [
            new ConcatenatedScriptPart(() => "always"),
            new ConcatenatedScriptPart(() => "conditional", hasPermission: () => granted),
            new ConcatenatedScriptPart(() => "nested", getCacheSuffix: () => nestedSuffix)
        ]);

        var first = script.CacheSuffix;
        granted = false;
        var second = script.CacheSuffix;
        nestedSuffix = "y";
        var third = script.CacheSuffix;

        Assert.NotEqual(first, second);
        Assert.NotEqual(second, third);
        Assert.NotEqual(first, third);
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
    public void ConcatenatedScript_HasPermission_Uses_Own_Permission_Only()
    {
        var script = new ConcatenatedScript(
            [new ConcatenatedScriptPart(() => "x", hasPermission: () => false)])
        {
            Permission = SpecialPermissionKeys.Public
        };
        Assert.True(script.HasPermission(new MockPermissions(x => x == SpecialPermissionKeys.Public)));
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
