namespace Serenity.Web;

public class RecaptchaSettingsTests
{
    [Fact]
    public void SectionKey_IsRecaptcha()
    {
        Assert.Equal("Recaptcha", RecaptchaSettings.SectionKey);
    }

    [Fact]
    public void SiteKey_GetSet_Works()
    {
        var settings = new RecaptchaSettings { SiteKey = "site-key" };
        Assert.Equal("site-key", settings.SiteKey);
    }

    [Fact]
    public void SecretKey_GetSet_Works()
    {
        var settings = new RecaptchaSettings { SecretKey = "secret-key" };
        Assert.Equal("secret-key", settings.SecretKey);
    }
}