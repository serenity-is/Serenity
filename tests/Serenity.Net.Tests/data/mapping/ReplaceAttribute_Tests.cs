namespace Serenity.Data.Mapping;

// The results of these expressions are checked against real databases by ExpressionAttributeIntegrationTests
public class ReplaceAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_An_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ReplaceAttribute(null!, "'a'", "'b'"));
        Assert.Throws<ArgumentNullException>(() => new ReplaceAttribute("S", null!, "'b'"));
        Assert.Throws<ArgumentNullException>(() => new ReplaceAttribute("S", "'a'", null!));
    }

    [Fact]
    public void Is_The_Same_For_Each_Dialect_But_Casts_Parameters_For_Firebird()
    {
        Assert.Equal("REPLACE(S, 'a', 'b')", new ReplaceAttribute("S", "'a'", "'b'").ToString(PostgresDialect.Instance));
        Assert.Equal("REPLACE(S, 'a', 'b')", new ReplaceAttribute("S", "'a'", "'b'").ToString(FirebirdDialect.Instance));
        Assert.Equal("REPLACE(S, CAST(@a AS VARCHAR(8191)), 'b')",
            new ReplaceAttribute("S", "@a", "'b'").ToString(FirebirdDialect.Instance));
    }
}
