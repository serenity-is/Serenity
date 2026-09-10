namespace Serenity.Data;

public class ConnectionStringsExtensionsTests
{
    [Fact]
    public void Get_NullConnectionStrings_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IConnectionStrings)null!).Get("Default"));
    }

    [Fact]
    public void Get_UnknownKey_ThrowsArgumentOutOfRange()
    {
        var options = new ConnectionStringOptions();
        var cs = new DefaultConnectionStrings(options);

        Assert.Throws<ArgumentOutOfRangeException>(() => cs.Get("Default"));
    }

    [Fact]
    public void Get_ReturnsConnectionString()
    {
        var options = new ConnectionStringOptions
        {
            ["Default"] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient"
            }
        };
        var cs = new DefaultConnectionStrings(options);

        Assert.Equal("Default", cs.Get("Default").ConnectionKey);
    }
}
