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
        => Assert.Throws<ArgumentNullException>(() => new ConnectionKeyFallbackAttribute(null, "Default"));

    [Fact]
    public void Attribute_Ctor_Throws_OnEmptyKey()
        => Assert.Throws<ArgumentException>(() => new ConnectionKeyFallbackAttribute("", "Default"));

    [Fact]
    public void Attribute_Ctor_Throws_OnEmptyFallback()
        => Assert.Throws<ArgumentException>(() => new ConnectionKeyFallbackAttribute("AnotherKey", ""));

    [Fact]
    public void GetConnectionKeyFallbacks_ReturnsDeclaredChain()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal(["AnotherKey", "Default"], cs.GetConnectionKeyFallbacks("AnotherKey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_ReturnsOnlySelf_WhenNoFallback()
    {
        var cs = Create(["Default"]);
        Assert.Equal(["Default"], cs.GetConnectionKeyFallbacks("Default"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_MultiLevel()
    {
        var cs = Create(["Default"],
            new ConnectionKeyFallbackAttribute("SomeKey", "AnotherKey"),
            new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal(["SomeKey", "AnotherKey", "Default"],
            cs.GetConnectionKeyFallbacks("SomeKey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_IsCaseInsensitive()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal(["anotherkey", "Default"], cs.GetConnectionKeyFallbacks("anotherkey"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_Throws_OnCycle()
    {
        var cs = Create(["Default"],
            new ConnectionKeyFallbackAttribute("A", "B"),
            new ConnectionKeyFallbackAttribute("B", "A"));
        var ex = Assert.Throws<InvalidOperationException>(() => cs.GetConnectionKeyFallbacks("A"));
        Assert.Contains("cycle", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Methods_Throw_OnNullKey()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Throws<ArgumentNullException>(() => cs.GetConnectionKeyFallbacks(null));
        Assert.Throws<ArgumentNullException>(() => cs.ResolveConnectionKey(null));
        Assert.Throws<ArgumentNullException>(() => cs.GetConnectionKeysResolvingTo(null));
    }

    [Fact]
    public void Methods_Throw_OnEmptyKey()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Throws<ArgumentException>(() => cs.GetConnectionKeyFallbacks(""));
        Assert.Throws<ArgumentException>(() => cs.ResolveConnectionKey(""));
        Assert.Throws<ArgumentException>(() => cs.GetConnectionKeysResolvingTo(""));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsConfiguredKey_WhenNoFallback()
    {
        var cs = Create(["Default"]);
        Assert.Equal("Default", cs.ResolveConnectionKey("Default"));
    }

    [Fact]
    public void ResolveConnectionKey_UsesFallback_WhenLogicalKeyNotConfigured()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal("Default", cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsLogicalKey_WhenItIsConfigured()
    {
        var cs = Create(["Default", "AnotherKey"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal("AnotherKey", cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_ReturnsNull_WhenNoneConfigured()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Null(cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void ResolveConnectionKey_IsCaseInsensitive()
    {
        var cs = Create(["Default", "ANOTHERKEY"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        Assert.Equal("anotherkey", cs.ResolveConnectionKey("anotherkey"));
    }

    [Fact]
    public void BuildFallbackMap_LastDeclarationWins()
    {
        var cs = Create(["Default", "Other"],
            new ConnectionKeyFallbackAttribute("AnotherKey", "Default"),
            new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Equal("Other", cs.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_ReturnsConfiguredKeys()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        var result = cs.GetConnectionKeysResolvingTo("Default").OrderBy(x => x).ToArray();
        Assert.Equal(["AnotherKey", "Default"], result);
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_ExcludesKeysResolvingToConfiguredSelf()
    {
        var cs = Create(["Default", "AnotherKey"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        var result = cs.GetConnectionKeysResolvingTo("Default").OrderBy(x => x).ToArray();
        Assert.Equal(["Default"], result);
    }

    [Fact]
    public void TryGetConnectionString_ResolvesFallback_AndReturnsRegisteredKey()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        var info = cs.TryGetConnectionString("AnotherKey");
        Assert.NotNull(info);
        Assert.Equal("Default", info.ConnectionKey);
        Assert.Equal("Server=.;Database=Test;", info.ConnectionString);
    }

    [Fact]
    public void TryGetConnectionString_ReturnsNull_WhenNoFallbackOrConfig()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Other"));
        Assert.Null(cs.TryGetConnectionString("AnotherKey"));
    }

    [Fact]
    public void ListConnectionStrings_DoesNotIncludeLogicalFallbackKeys()
    {
        var cs = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        var keys = cs.ListConnectionStrings().Select(x => x.ConnectionKey).ToArray();
        Assert.Equal(["Default"], keys);
    }

    [Fact]
    public void DefaultSqlConnections_Degrades_WhenInnerDoesNotSupportFallbacks()
    {
        var sqlConnections = new DefaultSqlConnections(new BasicConnectionStrings("Default"));
        var fallbacks = Assert.IsType<IConnectionKeyFallbacks>(sqlConnections, exactMatch: false);

        Assert.Equal(["AnotherKey"], fallbacks.GetConnectionKeyFallbacks("AnotherKey"));
        Assert.Equal("Default", fallbacks.ResolveConnectionKey("Default"));
        Assert.Null(fallbacks.ResolveConnectionKey("Other"));
        Assert.Equal(["Default"], fallbacks.GetConnectionKeysResolvingTo("Default"));
    }

    [Fact]
    public void DefaultSqlConnections_Forwards_WhenInnerSupportsFallbacks()
    {
        var inner = Create(["Default"], new ConnectionKeyFallbackAttribute("AnotherKey", "Default"));
        var sqlConnections = new DefaultSqlConnections(inner);
        var fallbacks = Assert.IsType<IConnectionKeyFallbacks>(sqlConnections, exactMatch: false);

        Assert.Equal(["AnotherKey", "Default"], fallbacks.GetConnectionKeyFallbacks("AnotherKey"));
        Assert.Equal("Default", fallbacks.ResolveConnectionKey("AnotherKey"));
    }

    [Fact]
    public void GetFallbackMap_BuildsLazilyAndCaches()
    {
        var cs = new CountingConnectionStrings(Options.Create(TestOptions(["Default"])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", "Default")));

        Assert.Equal(0, cs.BuildCount);
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);
    }

    [Fact]
    public void GetFallbackMap_InvalidateRebuilds()
    {
        var cs = new CountingConnectionStrings(Options.Create(TestOptions(["Default"])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", "Default")));

        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(1, cs.BuildCount);

        cs.Invalidate();
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(2, cs.BuildCount);
    }

    [Fact]
    public void GetFallbackMap_OverrideCanAvoidCaching()
    {
        var cs = new NoCacheConnectionStrings(Options.Create(TestOptions(["Default"])),
            new FakeTypeSource(new ConnectionKeyFallbackAttribute("AnotherKey", "Default")));

        cs.GetConnectionKeyFallbacks("AnotherKey");
        cs.GetConnectionKeyFallbacks("AnotherKey");
        Assert.Equal(2, cs.BuildCount);
    }
}
