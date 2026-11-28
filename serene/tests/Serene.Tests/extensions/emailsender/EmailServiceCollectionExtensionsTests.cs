namespace Serenity.Extensions.DependencyInjection;

public class EmailServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEmailSender_Registers_Sender_As_Singleton()
    {
        var services = new ServiceCollection();
        var result = services.AddEmailSender();

        Assert.Same(services, result);
        Assert.Contains(services, x =>
            x.ServiceType == typeof(IEmailSender) &&
            x.ImplementationType == typeof(EmailSender) &&
            x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddEmailSender_Registers_The_Razor_Renderer_Of_The_Emails_Built_By_Code()
    {
        var services = new ServiceCollection();
        services.AddEmailSender();

        Assert.Contains(services, x =>
            x.ServiceType == typeof(IEmailMessageRenderer) &&
            x.ImplementationType == typeof(RazorEmailMessageRenderer) &&
            x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddEmailSender_Does_Not_Replace_Existing_Registration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IEmailSender>(new NullEmailSender());

        services.AddEmailSender();

        Assert.DoesNotContain(services, x =>
            x.ServiceType == typeof(IEmailSender) &&
            x.ImplementationType == typeof(EmailSender));
    }

    private class NullEmailSender : IEmailSender
    {
        public void Send(MimeKit.MimeMessage message, bool skipQueue = false)
        {
        }
    }
}
