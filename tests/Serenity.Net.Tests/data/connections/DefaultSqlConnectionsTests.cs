using System.Data.Common;

namespace Serenity.Data;

public class DefaultSqlConnectionsTests
{
    private class TestDbProviderFactory : DbProviderFactory
    {
        public bool ThrowOnConnect { get; set; }

        public override DbConnection CreateConnection() =>
            ThrowOnConnect ? new ThrowingConnection() : new MockDbConnection();
    }

    private class ThrowingConnection : MockDbConnection
    {
        public override string ConnectionString
        {
            get => "";
            set => throw new InvalidOperationException("nope");
        }
    }

    private class TestProfiler : IConnectionProfiler
    {
        public bool Called { get; private set; }

        public IDbConnection Profile(IDbConnection connection)
        {
            Called = true;
            return connection;
        }
    }

    private class SimpleConnectionStrings(Dictionary<string, IConnectionString> items) : IConnectionStrings
    {
        public IConnectionString? TryGetConnectionString(string connectionKey) =>
            items.TryGetValue(connectionKey, out var value) ? value : null;

        public IEnumerable<IConnectionString> ListConnectionStrings() => items.Values;
    }

    private static string RegisterProvider(bool throwOnConnect = false)
    {
        var name = "TestProvider_" + Guid.NewGuid().ToString("N");
        DbProviderFactories.RegisterFactory(name, new TestDbProviderFactory { ThrowOnConnect = throwOnConnect });
        return name;
    }

    private static IConnectionString Info(string key, string? connectionString = "cs",
        string? providerName = "p") =>
        new ConnectionStringInfo(key, connectionString, providerName, SqlServer2012Dialect.Instance);

    private static DefaultSqlConnections CreateSubtle(Dictionary<string, IConnectionString>? entries = null,
        IConnectionProfiler? profiler = null)
    {
        var items = entries ?? new()
        {
            ["Default"] = Info("Default")
        };
        return new DefaultSqlConnections(new SimpleConnectionStrings(items), profiler);
    }

    [Fact]
    public void Constructor_NullConnectionStrings_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultSqlConnections(null!));
    }

    [Fact]
    public void ListConnectionStrings_ReturnsFromSource()
    {
        var connections = CreateSubtle();

        Assert.Equal(["Default"], connections.ListConnectionStrings().Select(x => x.ConnectionKey));
    }

    [Fact]
    public void CreateConnection_NullArguments_Throw()
    {
        var connections = CreateSubtle();

        Assert.Throws<ArgumentNullException>(() => connections.New("cs", null!, SqlServer2012Dialect.Instance));
        Assert.Throws<ArgumentNullException>(() => connections.New(null!, "p", SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void New_CreatesAndWrapsConnection()
    {
        var provider = RegisterProvider();
        var connections = CreateSubtle();

        var connection = connections.New("Data Source=test", provider, SqlServer2012Dialect.Instance);

        Assert.IsType<WrappedConnection>(connection);
        Assert.Equal("Data Source=test", connection.ConnectionString);
    }

    [Fact]
    public void New_WithProfiler_UsesProfiledConnection()
    {
        var provider = RegisterProvider();
        var profiler = new TestProfiler();
        var connections = CreateSubtle(profiler: profiler);

        var connection = connections.New("cs", provider, SqlServer2012Dialect.Instance);

        Assert.True(profiler.Called);
        Assert.IsType<WrappedConnection>(connection);
    }

    [Fact]
    public void New_WhenSettingConnectionStringThrows_DisposesAndRethrows()
    {
        var provider = RegisterProvider(throwOnConnect: true);
        var connections = CreateSubtle();

        Assert.Throws<InvalidOperationException>(() => connections.New("cs", provider, SqlServer2012Dialect.Instance));
    }

    [Fact]
    public void NewByKey_UsesResolvedConnectionInfo()
    {
        var provider = RegisterProvider();
        var connections = CreateSubtle(new()
        {
            ["Default"] = Info("Default", "cs", provider)
        });

        var connection = connections.NewByKey("Default");

        Assert.IsType<WrappedConnection>(connection);
    }

    [Fact]
    public void NewByKey_UnknownKey_Throws()
    {
        var connections = CreateSubtle();

        Assert.Throws<InvalidOperationException>(() => connections.NewByKey("Missing"));
    }

    [Fact]
    public void TryGetConnectionString_ReturnsInfoOrNull()
    {
        var connections = CreateSubtle();

        Assert.NotNull(connections.TryGetConnectionString("Default"));
        Assert.Null(connections.TryGetConnectionString("Missing"));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_WithoutFallbackSource_ReturnsSelf()
    {
        var connections = CreateSubtle();

        Assert.Equal(["Default"], connections.GetConnectionKeyFallbacks("Default"));
        Assert.Throws<ArgumentNullException>(() => connections.GetConnectionKeyFallbacks(null!));
        Assert.Throws<ArgumentException>(() => connections.GetConnectionKeyFallbacks(""));
    }

    [Fact]
    public void GetConnectionKeyFallbacks_WithFallbackSource_Delegates()
    {
        var options = new ConnectionStringOptions
        {
            ["Default"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "p",
                FallbackFor = "Base"
            }
        };

        var connections = new DefaultSqlConnections(new DefaultConnectionStrings(options));

        Assert.Contains("Default", connections.GetConnectionKeyFallbacks("Base"));
        Assert.True(connections.GetConnectionKeyFallbacks("Base").Any());
    }

    [Fact]
    public void ResolveConnectionKey_WithAndWithoutFallbackSource()
    {
        var withoutFallback = CreateSubtle();

        Assert.Equal("Default", withoutFallback.ResolveConnectionKey("Default"));
        Assert.Null(withoutFallback.ResolveConnectionKey("Missing"));
        Assert.Throws<ArgumentException>(() => withoutFallback.ResolveConnectionKey(""));

        var options = new ConnectionStringOptions
        {
            ["Default"] = new ConnectionStringEntry { ConnectionString = "cs", ProviderName = "p" }
        };
        var withFallback = new DefaultSqlConnections(new DefaultConnectionStrings(options));

        Assert.Equal("Default", withFallback.ResolveConnectionKey("Default"));
        Assert.Null(withFallback.ResolveConnectionKey("Missing"));
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_WithAndWithoutFallbackSource()
    {
        var withoutFallback = CreateSubtle();

        Assert.Equal(["Default"], withoutFallback.GetConnectionKeysResolvingTo("Default"));
        Assert.Empty(withoutFallback.GetConnectionKeysResolvingTo("Missing"));
        Assert.Throws<ArgumentException>(() => withoutFallback.GetConnectionKeysResolvingTo(""));

        var options = new ConnectionStringOptions
        {
            ["Default"] = new ConnectionStringEntry { ConnectionString = "cs", ProviderName = "p" }
        };
        var withFallback = new DefaultSqlConnections(new DefaultConnectionStrings(options));

        Assert.Equal(["Default"], withFallback.GetConnectionKeysResolvingTo("Default"));
        Assert.Empty(withFallback.GetConnectionKeysResolvingTo("Missing"));
    }
}
