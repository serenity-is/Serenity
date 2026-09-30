namespace Serenity.Data;

public class ParamPrefixReplacerTests
{
    [Fact]
    public void Replace_Null_Returns_Null()
    {
        Assert.Null(ParamPrefixReplacer.Replace(null, ':'));
    }

    [Fact]
    public void Replace_Empty_Returns_Empty()
    {
        Assert.Equal("", ParamPrefixReplacer.Replace("", ':'));
    }

    [Fact]
    public void Replace_Without_AtSign_Is_Unchanged()
    {
        Assert.Equal("select 1 where x = 5",
            ParamPrefixReplacer.Replace("select 1 where x = 5", ':'));
    }

    [Fact]
    public void Replace_Replaces_AtSign_With_Prefix()
    {
        Assert.Equal("x = :p1",
            ParamPrefixReplacer.Replace("x = @p1", ':'));
    }

    [Fact]
    public void Replace_Replaces_All_AtSigns()
    {
        Assert.Equal(":a + :b = :c",
            ParamPrefixReplacer.Replace("@a + @b = @c", ':'));
    }

    [Fact]
    public void Replace_AtSign_At_Start_And_End()
    {
        Assert.Equal(":p", ParamPrefixReplacer.Replace("@p", ':'));
        Assert.Equal("x:", ParamPrefixReplacer.Replace("x@", ':'));
    }

    [Fact]
    public void Replace_Does_Not_Replace_AtSign_In_Quotes()
    {
        Assert.Equal("x = '@p1'",
            ParamPrefixReplacer.Replace("x = '@p1'", ':'));
    }

    [Fact]
    public void Replace_Is_Quote_Aware()
    {
        Assert.Equal("a '@x' :p1",
            ParamPrefixReplacer.Replace("a '@x' @p1", ':'));
    }

    [Fact]
    public void Replace_With_At_Prefix_Is_Unchanged()
    {
        Assert.Equal("x = @p1",
            ParamPrefixReplacer.Replace("x = @p1", '@'));
    }

    [Fact]
    public void Replace_Does_Not_Touch_Double_AtSign()
    {
        // "@@" denotes server variables (e.g. T-SQL @@ROWCOUNT), not parameters.
        Assert.Equal("IF @@ROWCOUNT = 0",
            ParamPrefixReplacer.Replace("IF @@ROWCOUNT = 0", ':'));
        Assert.Equal("IF @@ROWCOUNT = 0",
            ParamPrefixReplacer.Replace("IF @@ROWCOUNT = 0", '@'));
        Assert.Equal("x = :p1 AND @@ROWCOUNT = 0",
            ParamPrefixReplacer.Replace("x = @p1 AND @@ROWCOUNT = 0", ':'));
    }

    [Fact]
    public void Replace_Does_Not_Translate_Inside_Line_Comments()
    {
        Assert.Equal("x = :p1 -- filter @p2",
            ParamPrefixReplacer.Replace("x = @p1 -- filter @p2", ':'));
    }

    [Fact]
    public void Replace_Apostrophe_In_Comment_Does_Not_Poison_Rest()
    {
        // The apostrophe in "don't" must not corrupt string tracking:
        // @p2 below is real SQL and has to be translated.
        Assert.Equal("x = :p1 -- don't\nAND y = :p2",
            ParamPrefixReplacer.Replace("x = @p1 -- don't\nAND y = @p2", ':'));
        Assert.Equal("x = :p1 -- don't\r\nAND y = :p2",
            ParamPrefixReplacer.Replace("x = @p1 -- don't\r\nAND y = :p2", ':'));
    }

    [Fact]
    public void Replace_DashDash_Inside_String_Is_Not_A_Comment()
    {
        Assert.Equal("x = '--' AND y = :p1",
            ParamPrefixReplacer.Replace("x = '--' AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Does_Not_Translate_Inside_Block_Comments()
    {
        Assert.Equal("x = :p1 /* filter @p2 */ AND y = :p3",
            ParamPrefixReplacer.Replace("x = @p1 /* filter @p2 */ AND y = @p3", ':'));
    }

    [Fact]
    public void Replace_Unclosed_Block_Comment_Swallow_Rest()
    {
        Assert.Equal("x = :p1 /* comment @p2",
            ParamPrefixReplacer.Replace("x = @p1 /* comment @p2", ':'));
    }

    [Fact]
    public void Replace_Block_Comment_Start_Inside_String_Is_Not_A_Comment()
    {
        Assert.Equal("x = '/*' AND y = :p1",
            ParamPrefixReplacer.Replace("x = '/*' AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Block_Comment_Start_Inside_Line_Comment_Is_Not_A_Comment()
    {
        Assert.Equal("x = :p1 -- /* @p2",
            ParamPrefixReplacer.Replace("x = @p1 -- /* @p2", ':'));
    }

    [Fact]
    public void Replace_Block_Comments_Do_Not_Nest()
    {
        // First */ closes; the rest is code again.
        Assert.Equal("x = :p1 /* a /* b @p2 */ AND y = :p3",
            ParamPrefixReplacer.Replace("x = @p1 /* a /* b @p2 */ AND y = @p3", ':'));
    }

    [Fact]
    public void Replace_Quote_And_DashDash_Inside_Block_Comment_Are_Inert()
    {
        Assert.Equal("/* it's -- @a */ AND y = :p1",
            ParamPrefixReplacer.Replace("/* it's -- @a */ AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Division_Slash_Is_Untouched()
    {
        Assert.Equal("x = a/b AND y = :p1",
            ParamPrefixReplacer.Replace("x = a/b AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Lone_Carriage_Return_Does_Not_End_Line_Comment()
    {
        // Only '\n' ends a line comment; a stray '\r' mid-line does not.
        Assert.Equal("x = :p1 -- a\rb = @p2",
            ParamPrefixReplacer.Replace("x = @p1 -- a\rb = @p2", ':'));
    }

    [Fact]
    public void Replace_Does_Not_Translate_Inside_Double_Quotes()
    {
        Assert.Equal("x = \"a@b\" AND y = :p1",
            ParamPrefixReplacer.Replace("x = \"a@b\" AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Does_Not_Translate_Inside_Backticks()
    {
        Assert.Equal("x = `a@b` AND y = :p1",
            ParamPrefixReplacer.Replace("x = `a@b` AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Apostrophe_In_Double_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("\"it's @a\" AND y = :p1",
            ParamPrefixReplacer.Replace("\"it's @a\" AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Double_Quote_In_Single_Quotes_Does_Not_End_Quote()
    {
        Assert.Equal("'say \"hi\" @a' AND y = :p1",
            ParamPrefixReplacer.Replace("'say \"hi\" @a' AND y = @p1", ':'));
    }

    [Fact]
    public void Replace_Quotes_After_Line_Comment_Stay_In_Comment()
    {
        // A quote inside a comment must not start a quoted region.
        Assert.Equal("-- it's @a\nAND y = :p1",
            ParamPrefixReplacer.Replace("-- it's @a\nAND y = @p1", ':'));
    }

    [Theory]
    [InlineData('\'')]
    [InlineData('"')]
    [InlineData('`')]
    public void Replace_Does_Not_Translate_Inside_Any_Quoted_Region(char quote)
    {
        var q = quote.ToString();
        Assert.Equal($"x = {q}a@b{q} AND y = :p1",
            ParamPrefixReplacer.Replace($"x = {q}a@b{q} AND y = @p1", ':'));
    }

    [Theory]
    [InlineData('\'')]
    [InlineData('"')]
    [InlineData('`')]
    public void Replace_Doubled_Quote_Stays_Inside_Region(char quote)
    {
        // '' (or "" / ``) is an escaped quote, not a region boundary.
        var q = quote.ToString();
        Assert.Equal($"x = {q}a{q}{q}b@c{q} AND y = :p1",
            ParamPrefixReplacer.Replace($"x = {q}a{q}{q}b@c{q} AND y = @p1", ':'));
    }

    [Theory]
    [InlineData('\'', '"')]
    [InlineData('\'', '`')]
    [InlineData('"', '\'')]
    [InlineData('"', '`')]
    [InlineData('`', '\'')]
    [InlineData('`', '"')]
    public void Replace_Other_Quote_Type_Does_Not_End_Region(char outer, char inner)
    {
        var o = outer.ToString();
        var n = inner.ToString();
        Assert.Equal($"x = {o}a{n}b@c{o} AND y = :p1",
            ParamPrefixReplacer.Replace($"x = {o}a{n}b@c{o} AND y = @p1", ':'));
    }

    [Theory]
    [InlineData('\'')]
    [InlineData('"')]
    [InlineData('`')]
    public void Replace_Unterminated_Region_Skips_Rest(char quote)
    {
        var q = quote.ToString();
        Assert.Equal($"x = {q}abc @p1",
            ParamPrefixReplacer.Replace($"x = {q}abc @p1", ':'));
    }
}
