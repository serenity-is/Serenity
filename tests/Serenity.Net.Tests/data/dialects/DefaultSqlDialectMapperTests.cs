namespace Serenity.Data;

public class DefaultSqlDialectMapperTests
{
    [Theory]
    [InlineData("System.Data.SqlClient", "SqlServer2012")]
    [InlineData("Npgsql", "Postgres")]
    public void TryGet_ByProviderName_ReturnsInstance(string providerName, string dialectName)
    {
        var mapper = new DefaultSqlDialectMapper();

        var dialect = mapper.TryGet(providerName)!;

        Assert.IsAssignableFrom<ISqlDialect>(dialect);
        Assert.Contains(dialectName, dialect.GetType().Name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryGet_ByDialectTypeName_CreatesInstance()
    {
        var mapper = new DefaultSqlDialectMapper();

        Assert.Equal(SqliteDialect.Instance.OpenQuote, mapper.TryGet("SqliteDialect")!.OpenQuote);
    }

    [Fact]
    public void TryGet_ByShortName_ReturnsDialect()
    {
        var mapper = new DefaultSqlDialectMapper();

        Assert.Equal(SqlServer2012Dialect.Instance.OpenQuote, mapper.TryGet("SqlServer2012")!.OpenQuote);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryGet_NullOrEmpty_ReturnsNull(string? name)
    {
        Assert.Null(new DefaultSqlDialectMapper().TryGet(name));
    }

    [Fact]
    public void TryGet_Unknown_ReturnsNull()
    {
        Assert.Null(new DefaultSqlDialectMapper().TryGet("NoSuchDialect"));
    }

    [Fact]
    public void TryGet_UpperCaseProviderName_ReturnsDialect()
    {
        Assert.Equal(PostgresDialect.Instance.OpenQuote, new DefaultSqlDialectMapper().TryGet("NPGSQL")!.OpenQuote);
    }
}
