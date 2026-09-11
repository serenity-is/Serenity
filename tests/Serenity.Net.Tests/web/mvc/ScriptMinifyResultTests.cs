namespace Serenity.Web;

public class ScriptMinifyResultTests
{
    [Fact]
    public void Properties_Can_Be_Set()
    {
        var result = new ScriptMinifyResult { Code = "var a=1;", HasErrors = true };

        Assert.Equal("var a=1;", result.Code);
        Assert.True(result.HasErrors);
    }

    [Fact]
    public void HasErrors_Defaults_To_False()
    {
        Assert.False(new ScriptMinifyResult { Code = "var a=1;" }.HasErrors);
    }
}
