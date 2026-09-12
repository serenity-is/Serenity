using MailKit.Security;

namespace Serenity.Extensions;

public class SmtpSettingsTests
{
    [Fact]
    public void SectionKey_Is_Expected()
    {
        Assert.Equal("SmtpSettings", SmtpSettings.SectionKey);
    }

    [Fact]
    public void Defaults_Are_Correct()
    {
        var settings = new SmtpSettings();
        Assert.Equal(SecureSocketOptions.Auto, settings.SecureSocket);
        Assert.False(settings.AutoUseQueue);
        Assert.Null(settings.Host);
        Assert.Null(settings.From);
        Assert.Null(settings.PickupPath);
    }

    [Fact]
    public void Properties_Can_Be_Set_And_Read()
    {
        var settings = new SmtpSettings
        {
            Host = "smtp.example.com",
            Port = 587,
            SecureSocket = SecureSocketOptions.StartTls,
            Username = "user",
            Password = "pass",
            From = "from@example.com",
            PickupPath = "Mail",
            AutoUseQueue = true
        };

        Assert.Equal("smtp.example.com", settings.Host);
        Assert.Equal(587, settings.Port);
        Assert.Equal(SecureSocketOptions.StartTls, settings.SecureSocket);
        Assert.Equal("user", settings.Username);
        Assert.Equal("pass", settings.Password);
        Assert.Equal("from@example.com", settings.From);
        Assert.Equal("Mail", settings.PickupPath);
        Assert.True(settings.AutoUseQueue);
    }
}
