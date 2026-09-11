namespace Serenity.Reporting;

public class WKHtmlToPdfSettingsTests
{
    [Fact]
    public void ExecutablePath_Can_Be_Set()
    {
        var settings = new WKHtmlToPdfSettings { ExecutablePath = "path" };
        Assert.Equal("path", settings.ExecutablePath);
    }
}
