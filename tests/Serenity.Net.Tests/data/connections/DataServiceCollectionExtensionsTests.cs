using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Serenity.Extensions.DependencyInjection;

public class DataServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSqlConnections_Registers_Defaults()
    {
        var services = new ServiceCollection();

        services.AddSqlConnections();

        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<ISqlConnections>());
        Assert.NotNull(sp.GetService<IConnectionStrings>());
        Assert.NotNull(sp.GetService<ISqlDialectMapper>());
    }

    [Fact]
    public void AddSqlConnections_With_SetupAction()
    {
        var services = new ServiceCollection();

        services.AddSqlConnections(_ => { });

        using var sp = services.BuildServiceProvider();
        Assert.NotNull(sp.GetService<IOptions<ConnectionStringOptions>>().Value);
    }

    [Fact]
    public void AddSqlConnections_Throws_ForNulls()
    {
        IServiceCollection services = null!;
        Assert.Throws<ArgumentNullException>(() => services.AddSqlConnections());

        var s2 = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => s2.AddSqlConnections(null!));
    }

    [Fact]
    public void GetDataConnectionString_Reads_Section()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Data:Default:ConnectionString"] = "cs",
                ["Data:Default:ProviderName"] = "pn",
                ["Data:Default:Dialect"] = "SqlServer"
            })
            .Build();

        var entry = config.GetDataConnectionString("Default");
        Assert.Equal("cs", entry.ConnectionString);
        Assert.Equal("pn", entry.ProviderName);
        Assert.Equal("SqlServer", entry.Dialect);
    }

    [Fact]
    public void GetDataConnectionString_Throws_When_No_ConnectionString()
    {
        var config = new ConfigurationBuilder().Build();
        Assert.Throws<ArgumentException>(() => config.GetDataConnectionString("Missing"));
    }
}
