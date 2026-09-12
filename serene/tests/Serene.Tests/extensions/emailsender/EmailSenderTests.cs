using MimeKit;

namespace Serenity.Extensions;

public class EmailSenderTests
{
    private class CaptureEmailQueue : IEmailQueue
    {
        public MimeMessage? Message { get; private set; }

        public void Enqueue(MimeMessage message)
        {
            Message = message;
        }
    }

    private static string CreateTempRoot()
    {
        return System.IO.Path.Combine(System.IO.Path.GetTempPath(),
            "serenity_email_" + Guid.NewGuid().ToString("N"));
    }

    private static MockHostEnvironment CreateHost(string root)
    {
        return new MockHostEnvironment(root);
    }

    [Fact]
    public void Constructor_Throws_For_Null_Arguments()
    {
        var host = CreateHost(CreateTempRoot());
        Assert.Throws<ArgumentNullException>(() =>
            new EmailSender(null!, Options.Create(new SmtpSettings())));
        Assert.Throws<ArgumentNullException>(() =>
            new EmailSender(host, null!));
    }

    [Fact]
    public void Send_Throws_For_Null_Message()
    {
        var sender = new EmailSender(CreateHost(CreateTempRoot()),
            Options.Create(new SmtpSettings()));
        Assert.Throws<ArgumentNullException>(() => sender.Send(null!, false));
    }

    [Fact]
    public void Send_Adds_From_Address_From_Settings_And_Enqueues()
    {
        var queue = new CaptureEmailQueue();
        var settings = new SmtpSettings { From = "from@example.com", AutoUseQueue = true };
        var sender = new EmailSender(CreateHost(CreateTempRoot()), Options.Create(settings), queue);
        var message = new MimeMessage();
        message.To.Add(MailboxAddress.Parse("to@example.com"));

        sender.Send(message, false);

        Assert.Equal("from@example.com", Assert.Single(message.From.Mailboxes).Address);
        Assert.Same(message, queue.Message);
    }

    [Fact]
    public void Send_Writes_Pickup_File_When_No_Host()
    {
        var root = CreateTempRoot();
        var sender = new EmailSender(CreateHost(root), Options.Create(new SmtpSettings()));
        var message = new MimeMessage();
        message.To.Add(MailboxAddress.Parse("to@example.com"));

        try
        {
            sender.Send(message, false);

            var pickupPath = System.IO.Path.Combine(root, "App_Data", "Mail");
            Assert.True(System.IO.Directory.Exists(pickupPath));
            Assert.NotEmpty(System.IO.Directory.GetFiles(pickupPath, "*.eml"));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Send_Writes_To_Custom_Pickup_Path_When_SkipQueue()
    {
        var root = CreateTempRoot();
        var settings = new SmtpSettings { AutoUseQueue = true, PickupPath = "CustomMail" };
        var sender = new EmailSender(CreateHost(root), Options.Create(settings), new CaptureEmailQueue());
        var message = new MimeMessage();
        message.To.Add(MailboxAddress.Parse("to@example.com"));

        try
        {
            sender.Send(message, skipQueue: true);

            var pickupPath = System.IO.Path.Combine(root, "CustomMail");
            Assert.True(System.IO.Directory.Exists(pickupPath));
            Assert.NotEmpty(System.IO.Directory.GetFiles(pickupPath, "*.eml"));
        }
        finally
        {
            if (System.IO.Directory.Exists(root))
                System.IO.Directory.Delete(root, true);
        }
    }
}

