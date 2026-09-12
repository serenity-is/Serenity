namespace Serenity.Extensions;

public class MembershipSettingsTests
{
    [Fact]
    public void SectionKey_Is_Expected()
    {
        Assert.Equal("Membership", MembershipSettings.SectionKey);
    }

    [Fact]
    public void Defaults_Are_Correct()
    {
        var settings = new MembershipSettings();
        Assert.Equal(6, settings.MinPasswordLength);
        Assert.True(settings.RequireDigit);
        Assert.True(settings.RequireLowercase);
        Assert.True(settings.RequireNonAlphanumeric);
        Assert.True(settings.RequireUppercase);
        Assert.Equal(5, settings.SaltSize);
    }

    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var settings = new MembershipSettings
        {
            MinPasswordLength = 8,
            RequireDigit = false,
            RequireLowercase = false,
            RequireNonAlphanumeric = false,
            RequireUppercase = false,
            SaltSize = 10
        };

        Assert.Equal(8, settings.MinPasswordLength);
        Assert.False(settings.RequireDigit);
        Assert.False(settings.RequireLowercase);
        Assert.False(settings.RequireNonAlphanumeric);
        Assert.False(settings.RequireUppercase);
        Assert.Equal(10, settings.SaltSize);
    }
}
