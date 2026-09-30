namespace Serenity.Data;

public class SqlRegionScannerTests
{
    private static string ConsumeAll(string expression)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < expression.Length; i++)
        {
            var start = i;
            if (SqlRegionScanner.TrySkipQuotesAndComments(expression, ref i))
                sb.Append(expression, start, i - start + 1);
            else
                sb.Append(expression[i]);
        }
        return sb.ToString();
    }

    [Fact]
    public void TrySkip_PlainText_ReturnsFalse()
    {
        var index = 0;

        Assert.False(SqlRegionScanner.TrySkipQuotesAndComments("abc", ref index));
        Assert.Equal(0, index);
    }

    [Fact]
    public void TrySkip_SingleQuotedString_ConsumesThroughQuote()
    {
        var expression = "'ab' cd";
        var index = 0;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(3, index);
        // round-trips the whole string
        Assert.Equal(expression, ConsumeAll(expression));
    }

    [Fact]
    public void TrySkip_UnclosedQuote_ConsumesRest()
    {
        var expression = "x = 'abc";
        var index = 4;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(expression.Length - 1, index);
    }

    [Fact]
    public void TrySkip_LineComment_ConsumesThroughNewline()
    {
        var expression = "a -- b\nc";
        var index = 2;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(6, index);
        Assert.Equal(expression, ConsumeAll(expression));
    }

    [Fact]
    public void TrySkip_LineComment_AtEnd_ConsumesRest()
    {
        var expression = "a -- b";
        var index = 2;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(expression.Length - 1, index);
    }

    [Fact]
    public void TrySkip_BlockComment_ConsumesThroughClose()
    {
        var expression = "a /* b */ c";
        var index = 2;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(8, index);
        Assert.Equal(expression, ConsumeAll(expression));
    }

    [Fact]
    public void TrySkip_UnclosedBlockComment_ConsumesRest()
    {
        var expression = "a /* b";
        var index = 2;

        Assert.True(SqlRegionScanner.TrySkipQuotesAndComments(expression, ref index));
        Assert.Equal(expression.Length - 1, index);
    }

    [Fact]
    public void TrySkip_LoneSlash_ReturnsFalse()
    {
        var index = 2;

        Assert.False(SqlRegionScanner.TrySkipQuotesAndComments("a / b", ref index));
        Assert.Equal(2, index);
    }

    [Fact]
    public void TrySkip_LoneDash_ReturnsFalse()
    {
        var index = 2;

        Assert.False(SqlRegionScanner.TrySkipQuotesAndComments("a - b", ref index));
        Assert.Equal(2, index);
    }
}
