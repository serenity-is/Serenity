namespace Serenity.Data.Mapping;

public class TrimLeftAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TrimLeftAttribute(null!));
    }

    [Fact]
    public void Only_Trims_The_Start()
    {
        var attribute = new TrimLeftAttribute("S");

        Assert.True(attribute.Leading);
        Assert.False(attribute.Trailing);
        Assert.Equal("LTRIM(S)", attribute.ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("TRIM(LEADING FROM S)", attribute.ToString(FirebirdDialect.Instance));
    }
}
