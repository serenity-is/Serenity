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
}
