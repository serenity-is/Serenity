namespace Serenity.Data;

public class ConnectionStringsExtensionsTests
{
    [Fact]
    public void Get_NullConnectionStrings_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ((IConnectionStrings)null).Get(DefaultConnectionAttribute.Key));
    }

    [Fact]
    public void Get_UnknownKey_ThrowsArgumentOutOfRange()
    {
        var options = new ConnectionStringOptions();
        var cs = new DefaultConnectionStrings(options);

        Assert.Throws<ArgumentOutOfRangeException>(() => cs.Get(DefaultConnectionAttribute.Key));
    }

    [Fact]
    public void Get_ReturnsConnectionString()
    {
        var options = new ConnectionStringOptions
        {
            [DefaultConnectionAttribute.Key] = new ConnectionStringEntry
            {
                ConnectionString = "cs",
                ProviderName = "System.Data.SqlClient"
            }
        };
        var cs = new DefaultConnectionStrings(options);

        Assert.Equal(DefaultConnectionAttribute.Key, cs.Get(DefaultConnectionAttribute.Key).ConnectionKey);
    }
}
