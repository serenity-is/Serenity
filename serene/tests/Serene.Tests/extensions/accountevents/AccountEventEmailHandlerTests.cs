using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
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

        public string ViewOf(IServiceProvider services, AccountEventKind kind) => GetViewName(services, kind);
    }

    /// <summary>The views of an application, by their paths.</summary>
    private sealed class Views(params string[] paths) : IRazorViewEngine
    {
        private sealed class View(string path) : IView
        {
            public string Path => path;
            public Task RenderAsync(ViewContext context) => Task.CompletedTask;
        }

        public ViewEngineResult GetView(string? executingFilePath, string viewPath, bool isMainPage) =>
            paths.Contains(viewPath) ? ViewEngineResult.Found(viewPath, new View(viewPath)) : ViewEngineResult.NotFound(viewPath, [viewPath]);
        public ViewEngineResult FindView(Microsoft.AspNetCore.Mvc.ActionContext context, string viewName, bool isMainPage) =>
            ViewEngineResult.NotFound(viewName, []);
        public RazorPageResult FindPage(Microsoft.AspNetCore.Mvc.ActionContext context, string pageName) => new(pageName, []);
        public RazorPageResult GetPage(string executingFilePath, string pagePath) => new(pagePath, []);
        public string? GetAbsolutePath(string? executingFilePath, string? pagePath) => pagePath;
    }

    [Fact]
    public void A_Kind_Of_Event_Has_A_View_Of_Its_Own_When_The_Application_Adds_One()
    {
        const string shared = "~/Areas/Serenity.Extensions/Membership/AccountEvents/AccountEventEmail.cshtml";
        const string own = "~/Areas/Serenity.Extensions/Membership/AccountEvents/AccountEventEmail.PasswordChanged.cshtml";
        var services = new ServiceCollection().AddSingleton<IRazorViewEngine>(new Views(own)).BuildServiceProvider();
        var handler = new TestHandler(new(), null);

        Assert.Equal(own, handler.ViewOf(services, AccountEventKind.PasswordChanged));
        Assert.Equal(shared, handler.ViewOf(services, AccountEventKind.EmailChanged));
        Assert.Equal(shared, handler.ViewOf(new ServiceCollection().BuildServiceProvider(), AccountEventKind.PasswordChanged));
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
