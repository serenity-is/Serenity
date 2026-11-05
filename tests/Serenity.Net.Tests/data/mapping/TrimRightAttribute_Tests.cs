namespace Serenity.Data.Mapping;

public class TrimRightAttribute_Tests
{
    [Fact]
    public void Throws_ArgumentNull_If_Expression_IsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new TrimRightAttribute(null!));
    }

    [Fact]
    public void Only_Trims_The_End()
    {
        var attribute = new TrimRightAttribute("S");

        Assert.False(attribute.Leading);
        Assert.True(attribute.Trailing);
        Assert.Equal("RTRIM(S)", attribute.ToString(SqlServer2012Dialect.Instance));
        Assert.Equal("TRIM(TRAILING FROM S)", attribute.ToString(FirebirdDialect.Instance));
    }
}
