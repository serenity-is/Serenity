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

    private class TestRow
    {
        [TrimLeft("X")]
        public string? Left { get; set; }

        [Trim("X", Leading = false)]
        public string? Right { get; set; }
    }

    [Fact]
    public void Can_Be_Used_On_A_Property_Including_Named_Arguments_Of_The_Base()
    {
        var left = typeof(TestRow).GetProperty(nameof(TestRow.Left))!.GetCustomAttribute<TrimAttribute>()!;
        var right = typeof(TestRow).GetProperty(nameof(TestRow.Right))!.GetCustomAttribute<TrimAttribute>()!;

        Assert.IsType<TrimLeftAttribute>(left);
        Assert.Equal("LTRIM(X)", left.ToString(PostgresDialect.Instance));
        Assert.Equal("RTRIM(X)", right.ToString(PostgresDialect.Instance));
    }
}
