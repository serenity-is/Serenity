using System.Data;

namespace Serenity.Data.Mapping;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
public class CastAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new CastAttribute(null!, DbType.Int32));
    }

    [Fact]
    public void Throws_For_Types_Without_An_Equivalent_In_All_Databases()
    {
        Assert.Throws<NotSupportedException>(() => new CastAttribute("X", DbType.Guid).ToString(PostgresDialect.Instance));
    }

    [Fact]
    public void Translates_Types_For_Each_Dialect()
    {
        Assert.Equal("CAST(X AS INT)", new CastAttribute("X", DbType.Int32).ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("CAST(X AS SIGNED)", new CastAttribute("X", DbType.Int64).ToString(MySqlDialect.Instance));
        Assert.Equal("CAST(X AS NUMBER(19))", new CastAttribute("X", DbType.Int64).ToString(OracleDialect.Instance));
        Assert.Equal("CAST(X AS DECIMAL(10, 3))", new CastAttribute("X", DbType.Decimal) { Precision = 10, Scale = 3 }.ToString(PostgresDialect.Instance));
        Assert.Equal("CAST(X AS REAL)", new CastAttribute("X", DbType.Decimal).ToString(SqliteDialect.Instance));
        Assert.Equal("CAST(X AS NVARCHAR(20))", new CastAttribute("X", DbType.String) { Size = 20 }.ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("CAST(X AS CHAR(20))", new CastAttribute("X", DbType.String) { Size = 20 }.ToString(MySqlDialect.Instance));
        Assert.Equal("CAST(X AS VARCHAR2(20 CHAR))", new CastAttribute("X", DbType.String) { Size = 20 }.ToString(OracleDialect.Instance));
    }

    [Fact]
    public void Drops_The_Time_For_Dates()
    {
        Assert.Equal("CAST(X AS DATE)", new CastAttribute("X", DbType.Date).ToString(PostgresDialect.Instance));
        Assert.Equal("DATE(X)", new CastAttribute("X", DbType.Date).ToString(SqliteDialect.Instance));
        Assert.Equal("TRUNC(CAST(X AS DATE))", new CastAttribute("X", DbType.Date).ToString(OracleDialect.Instance));
    }
}
