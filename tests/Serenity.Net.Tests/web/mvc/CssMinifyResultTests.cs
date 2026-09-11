namespace Serenity.Web;

public class CssMinifyResultTests
{
    [Fact]
    public void Properties_Can_Be_Set()
    {
        var result = new CssMinifyResult { Code = "body{}", HasErrors = true };

        Assert.Equal("body{}", result.Code);
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void HasErrors_Defaults_To_False()
    {
        Assert.False(new CssMinifyResult { Code = "body{}" }.HasErrors);
    }
}
