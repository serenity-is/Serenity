namespace Serenity.Data;

public class DefaultSqlDialectMapperTests
{
    [Theory]
    [InlineData("System.Data.SqlClient", "SqlServer2012")]
    [InlineData("Npgsql", "Postgres")]
    public void TryGet_ByProviderName_ReturnsInstance(string providerName, string dialectName)
    {
        var mapper = new DefaultSqlDialectMapper();

        var dialect = mapper.TryGet(providerName);

        Assert.IsAssignableFrom<ISqlDialect>(dialect);
        Assert.Contains(dialectName, dialect.GetType().Name, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryGet_ByDialectTypeName_ReturnsSingleton()
    {
        var mapper = new DefaultSqlDialectMapper();

        Assert.Same(SqliteDialect.Instance, mapper.TryGet("SqliteDialect"));
    }

    [Fact]
    public void TryGet_ByDialectTypeName_CachesInstance()
    {
        var mapper = new DefaultSqlDialectMapper();

        Assert.Same(mapper.TryGet("PostgresDialect"), mapper.TryGet("PostgresDialect"));
    }

    [Fact]
    public void TryGet_ByShortName_ReturnsDialect()
    {
        var mapper = new DefaultSqlDialectMapper();

        Assert.Same(SqlServer2012Dialect.Instance, mapper.TryGet("SqlServer2012"));
    }

    [Theory]
    [InlineData("SqlServer", typeof(SqlServer2012Dialect))]
    [InlineData("PostgreSQL", typeof(PostgresDialect))]
    public void TryGet_ByAlias_ReturnsDialect(string name, Type dialectType)
    {
        var dialect = new DefaultSqlDialectMapper().TryGet(name);

        Assert.IsType(dialectType, dialect);
    }

    [Theory]
    [InlineData("sqlitedialect", typeof(SqliteDialect))]
    [InlineData("SQLSERVER2012", typeof(SqlServer2012Dialect))]
    public void TryGet_TypeName_IsCaseInsensitive(string name, Type dialectType)
    {
        var dialect = new DefaultSqlDialectMapper().TryGet(name);

        Assert.IsType(dialectType, dialect);
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
        Assert.Equal(PostgresDialect.Instance.OpenQuote, new DefaultSqlDialectMapper().TryGet("NPGSQL").OpenQuote);
    }
}
