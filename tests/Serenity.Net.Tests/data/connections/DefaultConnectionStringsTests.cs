namespace Serenity.Data;

public class DefaultConnectionStringsTests
{
    private static DefaultConnectionStrings Create(
        Dictionary<string, ConnectionStringEntry>? entries = null)
    {
        var options = new ConnectionStringOptions();
        if (entries != null)
            foreach (var pair in entries)
                options.Add(pair.Key, pair.Value);

        return new DefaultConnectionStrings(options);
    }

    private static ConnectionStringEntry Entry(string? connectionString = "cs", string? providerName = "System.Data.SqlClient", string? dialect = null)
    {
        return new ConnectionStringEntry
        {
            ConnectionString = connectionString,
            ProviderName = providerName,
            Dialect = dialect
        };
    }

    [Fact]
    public void Constructor_NullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DefaultConnectionStrings(null!));
    }

    [Fact]
    public void TryGetConnectionString_ReturnsInfoAndCaches()
    {
        var settings = Create(new()
        {
            ["Default"] = Entry()
        });

        var info = settings.TryGetConnectionString("Default");
        Assert.NotNull(info);
        Assert.Equal("Default", info.ConnectionKey);
        Assert.Equal("cs", info.ConnectionString);
        Assert.Equal("System.Data.SqlClient", info.ProviderName);

        Assert.Same(info, settings.TryGetConnectionString("Default"));
    }

    [Fact]
    public void TryGetConnectionString_Unknown_ReturnsNull()
    {
        Assert.Null(Create().TryGetConnectionString("Unknown"));
    }

    [Fact]
    public void TryGetConnectionString_UsesDialectByName()
    {
        var settings = Create(new()
        {
            ["MySQL"] = Entry(dialect: "MySqlDialect")
        });

        var info = settings.TryGetConnectionString("MySQL");

        Assert.Equal(MySqlDialect.Instance.OpenQuote, info.Dialect.OpenQuote);
    }

    [Fact]
    public void TryGetConnectionString_UnknownDialectName_ThrowsArgumentException()
    {
        var settings = Create(new()
        {
            ["X"] = Entry(dialect: "NoSuchDialect")
        });

        Assert.Throws<ArgumentException>(() => settings.TryGetConnectionString("X"));
    }

    [Fact]
    public void TryGetConnectionString_WithDialectInstance_UsesInstance()
    {
        var dialect = OracleDialect.Instance;
        var settings = Create(new()
        {
            ["X"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient",
                DialectInstance = dialect
            }
        });

        Assert.Equal(dialect, settings.TryGetConnectionString("X")!.Dialect);
    }

    [Fact]
    public void TryGetConnectionString_MapsByProviderName()
    {
        var settings = Create(new()
        {
            ["Pg"] = Entry(providerName: "Npgsql")
        });

        Assert.Equal(PostgresDialect.Instance, settings.TryGetConnectionString("Pg")!.Dialect);
        Assert.Equal(MySqlDialect.Instance, Create(new()
        {
            ["My"] = Entry(providerName: "MySqlConnector")
        }).TryGetConnectionString("My")!.Dialect);
        Assert.Equal(FirebirdDialect.Instance, Create(new()
        {
            ["Fb"] = Entry(providerName: "FirebirdSql.Data.FirebirdClient")
        }).TryGetConnectionString("Fb")!.Dialect);
    }

    [Fact]
    public void TryGetConnectionString_UnknownProvider_ReturnsDefaultDialect()
    {
        var settings = Create(new()
        {
            ["U"] = Entry(providerName: "Unknown.Provider")
        });

        Assert.Equal(SqlSettings.DefaultDialect, settings.TryGetConnectionString("U")!.Dialect);
    }

    [Fact]
    public void ListConnectionStrings_ReturnsAllConfigured()
    {
        var settings = Create(new()
        {
            ["Default"] = Entry(),
            ["Second"] = Entry()
        });

        Assert.Equal(["Default", "Second"], settings.ListConnectionStrings()
            .Select(i => i.ConnectionKey).OrderBy(i => i).ToArray());
    }

    [Fact]
    public void ListConnectionStrings_MissingConnectionStringEntry_ISNull() 
    {
        var settings = Create(new()
        {
            ["Default"] = Entry(connectionString: null)
        });

        Assert.Throws<ArgumentNullException>(() => settings.ListConnectionStrings().ToList());
    }

    // Fallbacks via FallbackFor

    [Fact]
    public void ResolveConnectionKey_FallsBackToFallbackForOwner()
    {
        var options = new ConnectionStringOptions
        {
            ["Default"] = Entry(),
            ["ProFeatures"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient",
                FallbackFor = "Default"
            }
        };

        var cs = new DefaultConnectionStrings(options);

        Assert.Equal("ProFeatures", cs.ResolveConnectionKey("ProFeatures"));
        Assert.Null(cs.ResolveConnectionKey("Nothing"));
    }

    [Fact]
    public void ConnectionKeyFallbackCycle_ThrowsInvalidOperationException()
    {
        var options = new ConnectionStringOptions
        {
            ["A"] = new ConnectionStringEntry { ConnectionString = "cs", FallbackFor = "B" },
            ["B"] = new ConnectionStringEntry { ConnectionString = "cs", FallbackFor = "A" }
        };

        var cs = new DefaultConnectionStrings(options);

        Assert.Throws<InvalidOperationException>(() => cs.GetConnectionKeyFallbacks("A").ToList());
    }

    [Fact]
    public void FallbackForKeys_ParsesTrimmedKeys()
    {
        var entry = new ConnectionStringEntry { FallbackFor = " A ; B; " };

        Assert.Equal(new[] { "A", "B" }, entry.FallbackForKeys.OrderBy(k => k).ToArray());
        Assert.Same(entry.FallbackForKeys, entry.FallbackForKeys);
    }

    [Fact]
    public void FallbackForKeys_NullOrEmpty_ReturnsEmpty()
    {
        Assert.Empty(new ConnectionStringEntry().FallbackForKeys);
        Assert.Empty(new ConnectionStringEntry { FallbackFor = "" }.FallbackForKeys);
    }

    [Fact]
    public void GetConnectionKeysResolvingTo_ListsOwners()
    {
        var options = new ConnectionStringOptions
        {
            ["Default"] = Entry(),
            ["Pro"] = new ConnectionStringEntry { ConnectionString = "cs", FallbackFor = "Default" },
            ["Unrelated"] = Entry()
        };

        var cs = new DefaultConnectionStrings(options);

        Assert.Equal(["Default"], cs.GetConnectionKeysResolvingTo("Default").OrderBy(k => k).ToArray());
    }

    [Fact]
    public void ConnectionStringInfo_ConstructorArgumentChecks()
    {
        Assert.Throws<ArgumentNullException>(() => new ConnectionStringInfo(null!, "cs", "p", null!));
        Assert.Throws<ArgumentNullException>(() => new ConnectionStringInfo("k", "cs", "p", null!));
    }
}
