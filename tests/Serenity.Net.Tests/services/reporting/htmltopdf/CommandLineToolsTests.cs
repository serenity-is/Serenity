namespace Serenity.IO;

public class CommandLineToolsTests
{
    [Fact]
    public void EscapeArguments_Throws_For_Null_Argument()
    {
        Assert.Throws<ArgumentNullException>(() => CommandLineTools.EscapeArguments("a", null));
    }

    [Fact]
    public void EscapeArguments_Throws_For_Invalid_Characters()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CommandLineTools.EscapeArguments("a\nb"));
        Assert.Throws<ArgumentOutOfRangeException>(() => CommandLineTools.EscapeArguments("a\rb"));
        Assert.Throws<ArgumentOutOfRangeException>(() => CommandLineTools.EscapeArguments("a\0b"));
    }

    [Fact]
    public void EscapeArguments_Returns_Empty_For_No_Args()
    {
        Assert.Equal("", CommandLineTools.EscapeArguments());
    }

    [Fact]
    public void EscapeArguments_Leaves_Plain_Arguments_Unquoted()
    {
        Assert.Equal("simple", CommandLineTools.EscapeArguments("simple"));
    }

    [Fact]
    public void EscapeArguments_Quotes_Arguments_With_Spaces()
    {
        Assert.Equal("\"two words\"", CommandLineTools.EscapeArguments("two words"));
    }

    [Fact]
    public void EscapeArguments_Quotes_Empty_String()
    {
        Assert.Equal("\"\"", CommandLineTools.EscapeArguments(""));
    }

    [Fact]
    public void EscapeArguments_Escapes_Quotes()
    {
        Assert.Equal("\"a\\\"b\"", CommandLineTools.EscapeArguments("a\"b"));
    }

    [Fact]
    public void EscapeArguments_Joins_Arguments_With_Spaces()
    {
        Assert.Equal("a \"b c\"", CommandLineTools.EscapeArguments("a", "b c"));
    }
}
