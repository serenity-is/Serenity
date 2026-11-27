using MimeKit;
using System.Threading;

namespace Serenity.Extensions;

public class AccountEventEmailHandlerTests
{
    private sealed class CaptureEmailSender : IEmailSender
    {
        public List<MimeMessage> Messages { get; } = [];
        public void Send(MimeMessage message, bool skipQueue = false) => Messages.Add(message);
    }

    /// <summary>The handler, with the message of the email as its body, as the view is not rendered in a test.</summary>
    private sealed class TestHandler(AccountEventEmailOptions options, IEmailSender? sender)
        : AccountEventEmailHandler(Options.Create(options), NullTextLocalizer.Instance, null!, sender)
    {
        protected override string RenderBody(AccountEventEmailModel model) => model.Message;
    }

    private static AccountEvent Event(AccountEventKind kind) => new()
    {
        Kind = kind,
        UserId = "5",
        Username = "nancy",
        DisplayName = "Nancy",
        Email = "nancy@example.test"
    };

    [Fact]
    public async Task Emails_The_User_What_Changed()
    {
        var sender = new CaptureEmailSender();
        await new TestHandler(new(), sender).HandleAsync(Event(AccountEventKind.PasswordChanged), TestContext.Current.CancellationToken);

        var message = Assert.Single(sender.Messages);
        Assert.Equal("nancy@example.test", Assert.Single(message.To.Mailboxes).Address);
        Assert.Contains(AccountEventEmailTexts.PasswordChangedSubject.ToString(NullTextLocalizer.Instance), message.Subject);
    }

    [Fact]
    public async Task Emails_A_Changed_Email_Address_To_The_Old_One()
    {
        var sender = new CaptureEmailSender();
        var e = Event(AccountEventKind.EmailChanged);
        e.OldValue = "old@example.test";
        e.NewValue = "new@example.test";
        await new TestHandler(new(), sender).HandleAsync(e, TestContext.Current.CancellationToken);

        var message = Assert.Single(sender.Messages);
        Assert.Equal("old@example.test", Assert.Single(message.To.Mailboxes).Address);
        var body = Assert.IsType<TextPart>(message.Body).Text;
        Assert.Contains("old@example.test", body);
        Assert.Contains("new@example.test", body);
    }

    [Fact]
    public async Task Tells_The_Method_Of_Two_Factor_Authentication()
    {
        var sender = new CaptureEmailSender();
        var e = Event(AccountEventKind.TwoFactorEnabled);
        e.Method = "Authenticator";
        await new TestHandler(new(), sender).HandleAsync(e, TestContext.Current.CancellationToken);

        Assert.Contains("Authenticator", Assert.IsType<TextPart>(Assert.Single(sender.Messages).Body).Text);
    }

    [Fact]
    public async Task Sends_Nothing_When_Turned_Off_Skipped_Or_Without_An_Address_Or_Sender()
    {
        var sender = new CaptureEmailSender();
        await new TestHandler(new() { Enabled = false }, sender).HandleAsync(Event(AccountEventKind.PasswordChanged), TestContext.Current.CancellationToken);
        await new TestHandler(new() { Skip = "usernamechanged, TwoFactorEnabled" }, sender).HandleAsync(Event(AccountEventKind.UsernameChanged), TestContext.Current.CancellationToken);
        var noEmail = Event(AccountEventKind.PasswordChanged);
        noEmail.Email = null;
        await new TestHandler(new(), sender).HandleAsync(noEmail, TestContext.Current.CancellationToken);
        await new TestHandler(new(), null).HandleAsync(Event(AccountEventKind.PasswordChanged), TestContext.Current.CancellationToken);

        Assert.Empty(sender.Messages);
        Assert.True(new AccountEventEmailOptions { Skip = "UsernameChanged" }.IsEnabledFor(AccountEventKind.PasswordChanged));
    }

    [Fact]
    public async Task Sends_Nothing_For_The_Sign_Ins()
    {
        var sender = new CaptureEmailSender();
        foreach (var kind in new[] { AccountEventKind.SignedIn, AccountEventKind.SignInFailed, AccountEventKind.SignedOut,
            AccountEventKind.Elevated, AccountEventKind.ElevationFailed, AccountEventKind.PasswordResetRequested })
            await new TestHandler(new(), sender).HandleAsync(Event(kind), TestContext.Current.CancellationToken);

        Assert.Empty(sender.Messages);
    }
}
