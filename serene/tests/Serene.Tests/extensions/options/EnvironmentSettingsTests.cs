namespace Serenity.Extensions;

public class EnvironmentSettingsTests
{
    [Fact]
    public void SectionKey_Is_Expected()
    {
        Assert.Equal("EnvironmentSettings", EnvironmentSettings.SectionKey);
    }

    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var settings = new EnvironmentSettings
        {
            IsPublicDemo = true,
            SiteExternalUrl = "https://external",
            SiteInternalUrl = "https://internal",
            InjectMarkupToHead = "<meta>"
        };

        Assert.True(settings.IsPublicDemo);
        Assert.Equal("https://external", settings.SiteExternalUrl);
        Assert.Equal("https://internal", settings.SiteInternalUrl);
        Assert.Equal("<meta>", settings.InjectMarkupToHead);
    }
}
