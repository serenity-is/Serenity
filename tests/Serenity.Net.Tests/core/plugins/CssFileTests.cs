namespace Serenity.Plugins;

public class CssFileTests
{
    [Fact]
    public void Path_CanBeSet()
    {
        var cssFile = new CssFile()
        {
            Path = "path"
        };
        Assert.Equal("path", cssFile.Path);
    }

    [Fact]
    public void Path_CanBeSet_ToNull()
    {
        var cssFile = new CssFile()
        {
            Path = null
        };
        Assert.Null(cssFile.Path);
    }
}
