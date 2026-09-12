namespace Serenity.Extensions;

public class ClamAVSettingsTests
{
    [Fact]
    public void SectionKey_Is_Expected()
    {
        Assert.Equal("ClamAV", ClamAVSettings.SectionKey);
    }

    [Fact]
    public void Defaults_Are_Correct()
    {
        var settings = new ClamAVSettings();
        Assert.True(settings.Enabled);
        Assert.Equal("localhost", settings.Host);
        Assert.Equal(3310, settings.Port);
    }

    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var settings = new ClamAVSettings
        {
            Enabled = false,
            Host = "clam",
            Port = 1234
        };

        Assert.False(settings.Enabled);
        Assert.Equal("clam", settings.Host);
        Assert.Equal(1234, settings.Port);
    }
}
