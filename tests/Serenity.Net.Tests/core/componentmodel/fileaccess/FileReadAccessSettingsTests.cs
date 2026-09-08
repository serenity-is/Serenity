namespace Serenity.Web;

public class FileReadAccessSettingsTests
{
    [Fact]
    public void SectionKey_IsFileReadAccess()
    {
        Assert.Equal("FileReadAccess", FileReadAccessSettings.SectionKey);
    }

    [Fact]
    public void DefaultPermission_DefaultsToStar()
    {
        var settings = new FileReadAccessSettings();
        Assert.Equal("*", settings.DefaultPermission);
    }

    [Fact]
    public void PathPermissions_DefaultsToPublicAndTemporary()
    {
        var settings = new FileReadAccessSettings();
        Assert.Equal("^public/:*;^temporary/:*", settings.PathPermissions);
    }

    [Fact]
    public void BypassPermission_GetSet_Works()
    {
        var settings = new FileReadAccessSettings { BypassPermission = "Admin" };
        Assert.Equal("Admin", settings.BypassPermission);
    }

    [Fact]
    public void MissingMetadataPermission_GetSet_Works()
    {
        var settings = new FileReadAccessSettings { MissingMetadataPermission = "Meta" };
        Assert.Equal("Meta", settings.MissingMetadataPermission);
    }

    [Fact]
    public void EnableAccessLogging_GetSet_Works()
    {
        var settings = new FileReadAccessSettings { EnableAccessLogging = true };
        Assert.True(settings.EnableAccessLogging);
    }

    [Fact]
    public void ReturnForbidResult_GetSet_Works()
    {
        var settings = new FileReadAccessSettings { ReturnForbidResult = true };
        Assert.True(settings.ReturnForbidResult);
    }

    [Fact]
    public void Value_ReturnsSelf()
    {
        var settings = new FileReadAccessSettings();
        Assert.Same(settings, settings.Value);
    }
}