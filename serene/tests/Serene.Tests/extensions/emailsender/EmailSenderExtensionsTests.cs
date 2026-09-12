using MimeKit;

namespace Serenity.Extensions;

public class EmailSenderExtensionsTests
{
    private class CaptureEmailSender : IEmailSender
    {
        public MimeMessage? Message { get; private set; }

        public void Send(MimeMessage message, bool skipQueue = false)
        {
            Message = message;
        }
    }

    [Fact]
    public void Send_Builds_Message_With_Recipient_Subject_And_Body()
    {
        var sender = new CaptureEmailSender();
        sender.Send("Subject", "<b>body</b>", "to@example.com");

        var message = sender.Message.AssertNotNull();
        Assert.Equal("Subject", message.Subject);
        Assert.Equal("to@example.com", Assert.Single(message.To.Mailboxes).Address);
        var body = Assert.IsType<TextPart>(message.Body);
        Assert.Equal("<b>body</b>", body.Text);
    }

    [Fact]
    public void Send_Throws_For_Null_Recipient()
    {
        var sender = new CaptureEmailSender();
        Assert.Throws<ArgumentNullException>(() => sender.Send("Subject", "body", null!));
    }
}
