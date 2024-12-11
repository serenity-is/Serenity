namespace Serenity.Plugins;

public class ScriptFileTests
{
    [Fact]
    public void Path_CanBeSet()
    {
        var script = new ScriptFile()
        {
            Path = "text"
        };
        Assert.Equal("text", script.Path);
    }

    [Fact]
    public void Path_CanBeSet_Null()
    {
        var script = new ScriptFile()
        {
            Path = null
        };
        Assert.Null(script.Path);
    }
}