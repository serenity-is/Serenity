namespace Serenity.Web.EsBuild;

public class EsBuildPlatformInfoTests
{
    [Fact]
    public void Platform_Returns_Known_Value()
    {
        var platform = new EsBuildPlatformInfo().Platform;

        if (OperatingSystem.IsWindows())
            Assert.Equal("win32", platform);
        else if (OperatingSystem.IsMacOS())
            Assert.Equal("darwin", platform);
        else
            Assert.Equal("linux", platform);
    }

    [Fact]
    public void Architecture_Returns_Known_Value()
    {
        var architecture = new EsBuildPlatformInfo().Architecture;

        Assert.Contains(architecture, new[] { "x64", "x86", "arm64" });
    }
}
