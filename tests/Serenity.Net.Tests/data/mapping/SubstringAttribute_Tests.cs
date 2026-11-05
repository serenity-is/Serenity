namespace Serenity.Data.Mapping;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
public class SubstringAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_An_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SubstringAttribute(null!, 1, 2));
        Assert.Throws<ArgumentNullException>(() => new SubstringAttribute("S", null!, 2));
        Assert.Throws<ArgumentNullException>(() => new SubstringAttribute("S", 1, null!));
    }

    [Fact]
    public void Translates_For_Each_Dialect()
    {
        Assert.Equal("SUBSTRING(S, 1, 2)", new SubstringAttribute("S", 1, 2).ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("SUBSTRING(S, 1, 2)", new SubstringAttribute("S", 1, 2).ToString(PostgresDialect.Instance));
        Assert.Equal("SUBSTRING(S, 1, 2)", new SubstringAttribute("S", 1, 2).ToString(MySqlDialect.Instance));
        Assert.Equal("SUBSTR(S, 1, 2)", new SubstringAttribute("S", 1, 2).ToString(SqliteDialect.Instance));
        Assert.Equal("SUBSTR(S, 1, 2)", new SubstringAttribute("S", 1, 2).ToString(OracleDialect.Instance));
        Assert.Equal("SUBSTRING(S FROM 1 FOR 2)", new SubstringAttribute("S", 1, 2).ToString(FirebirdDialect.Instance));
    }

    [Fact]
    public void Casts_Parameters_For_Firebird()
    {
        Assert.Equal("SUBSTRING(CAST(@s AS VARCHAR(8191)) FROM CAST(@a AS INTEGER) FOR CAST(@b AS INTEGER))",
            new SubstringAttribute("@s", "@a", "@b").ToString(FirebirdDialect.Instance));
    }
}
