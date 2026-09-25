using Microsoft.Extensions.Options;

namespace Serenity.Data;

public class ConnectionKeyFallbackTests
{
    private static ConnectionStringOptions TestOptions(string[] configuredKeys)
    {
        var options = new ConnectionStringOptions();
        foreach (var key in configuredKeys)
        {
            options[key] = new ConnectionStringEntry
            {
                ConnectionString = "Server=.;Database=Test;",
                ProviderName = "System.Data.SqlClient"
            };
        }
        return options;
    }

    private static DefaultConnectionStrings Create(string[] configuredKeys, params Attribute[] attributes)
    {
        var typeSource = new FakeTypeSource(attributes);
        return new DefaultConnectionStrings(Options.Create(TestOptions(configuredKeys)), typeSource: typeSource);
    }

    private static DefaultConnectionStrings CreateWithFallbacks(Dictionary<string, string> fallbackFor, params Attribute[] attributes)
    {
        var options = new ConnectionStringOptions();
        foreach (var kv in fallbackFor)
        {
            options[kv.Key] = new ConnectionStringEntry
            {
                ConnectionString = "Server=.;Database=Test;",
                ProviderName = "System.Data.SqlClient",
                FallbackFor = kv.Value
            };
        }
        return new DefaultConnectionStrings(Options.Create(options), typeSource: new FakeTypeSource(attributes));
    }

    private sealed class FakeTypeSource(params Attribute[] attributes) : ITypeSource
    {
        private readonly Attribute[] attributes = attributes;

        public IEnumerable<Attribute> GetAssemblyAttributes(Type attributeType)
            => attributes.Where(a => attributeType.IsInstanceOfType(a));

        public IEnumerable<Type> GetTypes() => [];

        public IEnumerable<Type> GetTypesWithAttribute(Type attributeType) => [];

        public IEnumerable<Type> GetTypesWithInterface(Type interfaceType) => [];
    }

    private sealed class BasicConnectionStrings(params string[] keys) : IConnectionStrings
    {
        private readonly string[] keys = keys;

        public IConnectionString TryGetConnectionString(string connectionKey)
            => keys.Any(k => string.Equals(k, connectionKey, StringComparison.OrdinalIgnoreCase))
                ? new ConnectionStringInfo(connectionKey, "Server=.;Database=Test;",
                    "System.Data.SqlClient", SqlSettings.DefaultDialect)
                : null;

        public IEnumerable<IConnectionString> ListConnectionStrings()
            => keys.Select(TryGetConnectionString);
    }

    private sealed class CountingConnectionStrings(IOptions<ConnectionStringOptions> options, ITypeSource typeSource) : DefaultConnectionStrings(options, typeSource: typeSource)
    {
        public int BuildCount;

        public void Invalidate() => fallbackMap = null;

        protected override Dictionary<string, string> GetFallbackMap()
        {
            if (fallbackMap == null)
                BuildCount++;
            return base.GetFallbackMap();
        }
    }

    private sealed class NoCacheConnectionStrings(IOptions<ConnectionStringOptions> options, ITypeSource typeSource) : DefaultConnectionStrings(options, typeSource: typeSource)
    {
        private readonly ITypeSource typeSource = typeSource;
        public int BuildCount;

        protected override Dictionary<string, string> GetFallbackMap()
        {
            BuildCount++;
            return BuildFallbackMap(typeSource);
        }
    }

    [Fact]
    public void Attribute_Ctor_Throws_OnNullKey()
        => Assert.Throws<ArgumentNullException>(() => new ConnectionKeyFallbackAttribute(null, DefaultConnectionAttribute.Key));

    [Fact]
    public void Attribute_Ctor_Throws_OnEmptyKey()
        => Assert.Throws<ArgumentException>(() => new ConnectionKeyFallbackAttribute("", DefaultConnectionAttribute.Key));

    [Fact]
    public void Attribute_Ctor_Throws_OnEmptyFallback()
        => Assert.Throws<ArgumentException>(() => new ConnectionKeyFallbackAttribute("AnotherKey", ""));

    [Fact]
    public void GetConnectionKeyFallbacks_ReturnsDeclaredChain()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal(["AnotherKey", DefaultConnectionAttribute.Key], cs.GetConnectionKeyFallbacks("AnotherKey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_ReturnsOnlySelf_WhenNoFallback()
    {
        var cs = Create([DefaultConnectionAttribute.Key]);
        Assert.Equal([DefaultConnectionAttribute.Key], cs.GetConnectionKeyFallbacks(DefaultConnectionAttribute.Key));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_MultiLevel()
    {
        var cs = Create([DefaultConnectionAttribute.Key],
            new ConnectionKeyFallbackAttribute("SomeKey", "AnotherKey"),
            new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal(["SomeKey", "AnotherKey", DefaultConnectionAttribute.Key],
            cs.GetConnectionKeyFallbacks("SomeKey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_IsCaseInsensitive()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal(["anotherkey", DefaultConnectionAttribute.Key], cs.GetConnectionKeyFallbacks("anotherkey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_Throws_OnCycle()
    {
        var cs = Create([DefaultConnectionAttribute.Key],
            new ConnectionKeyFallbackAttribute("A", "B"),
            new ConnectionKeyFallbackAttribute("B", "A"));
        var ex = Assert.Throws<InvalidOperationException>(() => cs.GetConnectionKeyFallbacks("A"));
        Assert.Contains("cycle", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Methods_Throw_OnNullKey()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Throws<ArgumentNullException>(() => cs.GetConnectionKeyFallbacks(null));
        Assert.Throws<ArgumentNullException>(() => cs.ResolveConnectionKey(null));
        Assert.Throws<ArgumentNullException>(() => cs.GetConnectionKeysResolvingTo(null));
    }

    [Fact]
    public void Methods_Throw_OnEmptyKey()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Throws<ArgumentException>(() => cs.GetConnectionKeyFallbacks(""));
        Assert.Throws<ArgumentException>(() => cs.ResolveConnectionKey(""));
        Assert.Throws<ArgumentException>(() => cs.GetConnectionKeysResolvingTo(""));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsConfiguredKey_WhenNoFallback()
    {
        var cs = Create([DefaultConnectionAttribute.Key]);
        Assert.Equal(DefaultConnectionAttribute.Key, cs.ResolveConnectionKey(DefaultConnectionAttribute.Key));
    }

    [Fact]
    public void ResolveConnectionKey_UsesFallback_WhenLogicalKeyNotConfigured()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal(DefaultConnectionAttribute.Key, cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsLogicalKey_WhenItIsConfigured()
    {
        var cs = Create([DefaultConnectionAttribute.Key, "AnotherKey"], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal("AnotherKey", cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsNull_WhenNoneConfigured()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Null(cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_IsCaseInsensitive()
    {
        var cs = Create([DefaultConnectionAttribute.Key, "ANOTHERKEY"], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        Assert.Equal("anotherkey", cs.ResolveConnectionKey("anotherkey"));
    }

    [Fact]
    public void BuildFallbackMap_LastDeclarationWins()
    {
        var cs = Create([DefaultConnectionAttribute.Key, "Other"],
            new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key),
            new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Equal("Other", cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_ReturnsConfiguredKeys()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        var result = cs.GetConnectionKeysResolvingTo(DefaultConnectionAttribute.Key).OrderBy(x => x).ToArray();
        Assert.Equal(["AnotherKey", DefaultConnectionAttribute.Key], result);
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_ExcludesKeysResolvingToConfiguredSelf()
    {
        var cs = Create([DefaultConnectionAttribute.Key, "AnotherKey"], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        var result = cs.GetConnectionKeysResolvingTo(DefaultConnectionAttribute.Key).OrderBy(x => x).ToArray();
        Assert.Equal([DefaultConnectionAttribute.Key], result);
    }

    [Fact]
    public void TryGetConnectionString_ResolvesFallback_AndReturnsRegisteredKey()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        var info = cs.TryGetConnectionString("AnotherKey");
        Assert.NotNull(info);
        Assert.Equal(DefaultConnectionAttribute.Key, info.ConnectionKey);
        Assert.Equal("Server=.;Database=Test;", info.ConnectionString);
    }

    [Fact]
    public void TryGetConnectionString_ReturnsNull_WhenNoFallbackOrConfig()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Null(cs.TryGetConnectionString("AnotherKey"));
    }

    [Fact]
    public void ListConnectionStrings_DoesNotIncludeLogicalFallbackKeys()
    {
        var cs = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        var keys = cs.ListConnectionStrings().Select(x => x.ConnectionKey).ToArray();
        Assert.Equal([DefaultConnectionAttribute.Key], keys);
    }

    [Fact]
    public void DefaultSqlConnections_Degrades_WhenInnerDoesNotSupportFallbacks()
    {
        var sqlConnections = new DefaultSqlConnections(new BasicConnectionStrings(DefaultConnectionAttribute.Key));
        var fallbacks = Assert.IsType<IConnectionKeyFallbacks>(sqlConnections, exactMatch: false);

        Assert.Equal(["AnotherKey"], fallbacks.GetConnectionKeyFallbacks("AnotherKey"));
        Assert.Equal(DefaultConnectionAttribute.Key, fallbacks.ResolveConnectionKey(DefaultConnectionAttribute.Key));
        Assert.Null(fallbacks.ResolveConnectionKey("Other"));
        Assert.Equal([DefaultConnectionAttribute.Key], fallbacks.GetConnectionKeysResolvingTo(DefaultConnectionAttribute.Key));
    }

    [Fact]
    public void DefaultSqlConnections_Forwards_WhenInnerSupportsFallbacks()
    {
        var inner = Create([DefaultConnectionAttribute.Key], new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key));
        var sqlConnections = new DefaultSqlConnections(inner);
        var fallbacks = Assert.IsType<IConnectionKeyFallbacks>(sqlConnections, exactMatch: false);

        Assert.Equal(["AnotherKey", DefaultConnectionAttribute.Key], fallbacks.GetConnectionKeyFallbacks("AnotherKey"));
        Assert.Equal(DefaultConnectionAttribute.Key, fallbacks.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void GetFallbackMap_BuildsLazilyAndCaches()
    {
        var cs = new CountingConnectionStrings(Options.Create(TestOptions([DefaultConnectionAttribute.Key])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key)));

        Assert.Equal(0, cs.BuildCount);
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);
    }

    [Fact]
    public void GetFallbackMap_InvalidateRebuilds()
    {
        var cs = new CountingConnectionStrings(Options.Create(TestOptions([DefaultConnectionAttribute.Key])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key)));

        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);

        cs.Invalidate();
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(2, cs.BuildCount);
    }

    [Fact]
    public void GetFallbackMap_OverrideCanAvoidCaching()
    {
        var cs = new NoCacheConnectionStrings(Options.Create(TestOptions([DefaultConnectionAttribute.Key])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", DefaultConnectionAttribute.Key)));

        cs.GetConnectionKeyFallbacks("AnotherKey");
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(2, cs.BuildCount);
    }

    [Fact]
    public void FallbackFor_ConfigCreatesMapping()
    {
        var cs = CreateWithFallbacks(new() { [DefaultConnectionAttribute.Key] = "ProFeatures" });
        Assert.Equal(["ProFeatures", DefaultConnectionAttribute.Key], cs.GetConnectionKeyFallbacks("ProFeatures"));
        Assert.Equal(DefaultConnectionAttribute.Key, cs.ResolveConnectionKey("ProFeatures"));
        Assert.Equal(DefaultConnectionAttribute.Key, cs.TryGetConnectionString("ProFeatures")?.ConnectionKey);
    }

    [Fact]
    public void FallbackFor_ConfigOverridesAssemblyAttribute()
    {
        var cs = CreateWithFallbacks(new() { ["MyConn"] = "ProFeatures" },
            new ConnectionKeyFallbackAttribute("ProFeatures", DefaultConnectionAttribute.Key));
        Assert.Equal("MyConn", cs.ResolveConnectionKey("ProFeatures"));
    }

    [Fact]
    public void FallbackFor_MultipleSources()
    {
        var cs = CreateWithFallbacks(new() { ["MyConn"] = "ProFeatures;ProWorkLog" });
        Assert.Equal("MyConn", cs.ResolveConnectionKey("ProFeatures"));
        Assert.Equal("MyConn", cs.ResolveConnectionKey("ProWorkLog"));
    }

    [Fact]
    public void FallbackFor_IgnoresBlankAndWhitespace()
    {
        var cs = CreateWithFallbacks(new() { [DefaultConnectionAttribute.Key] = "ProFeatures; ;  " });
        Assert.Equal(DefaultConnectionAttribute.Key, cs.ResolveConnectionKey("ProFeatures"));
        Assert.Equal(["ProFeatures", DefaultConnectionAttribute.Key], cs.GetConnectionKeyFallbacks("ProFeatures"));
    }

    [Fact]
    public void FallbackFor_ConflictThrows()
    {
        var cs = CreateWithFallbacks(new()
        {
            [DefaultConnectionAttribute.Key] = "ProFeatures",
            ["MyConn"] = "ProFeatures"
        });
        var ex = Assert.Throws<InvalidOperationException>(() => cs.GetConnectionKeyFallbacks("ProFeatures"));
        Assert.Contains("conflict", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FallbackFor_DuplicateInSameEntry_NoConflict()
    {
        var cs = CreateWithFallbacks(new() { [DefaultConnectionAttribute.Key] = "ProFeatures;ProFeatures" });
        Assert.Equal(DefaultConnectionAttribute.Key, cs.ResolveConnectionKey("ProFeatures"));
    }

    [Fact]
    public void FallbackFor_LazyParseIsCachedOnEntry()
    {
        var entry = new ConnectionStringEntry { FallbackFor = "ProFeatures;ProWorkLog" };
        var first = entry.FallbackForKeys;
        Assert.Equal(2, first.Count);
        Assert.Same(first, entry.FallbackForKeys);
    }
}
