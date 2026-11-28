namespace Serenity.Extensions;

public class BasicEmailMessageRendererTests
{
    [Fact]
    public void Renders_The_Parts_Of_The_Message_Encoded()
    {
        var html = BasicEmailMessageRenderer.Instance.Render(new EmailMessageModel
        {
            Subject = "You <are> invited",
            Greeting = "Hello Nancy & co,",
            Paragraphs = ["Admin invited you."],
            Code = "123456",
            Button = new EmailButtonModel { Text = "Create your account", Url = "https://x.test/a?b=1&c=2" },
            Notes = ["The link works once."]
        });

        Assert.Contains("<title>You &lt;are&gt; invited</title>", html, StringComparison.Ordinal);
        Assert.Contains("<p>Hello Nancy &amp; co,</p>", html, StringComparison.Ordinal);
        Assert.Contains("<p>Admin invited you.</p>", html, StringComparison.Ordinal);
        Assert.Contains("123456", html, StringComparison.Ordinal);
        Assert.Contains("<a href=\"https://x.test/a?b=1&amp;c=2\">Create your account</a>", html, StringComparison.Ordinal);
        Assert.Contains("<p>The link works once.</p>", html, StringComparison.Ordinal);
    }

    [Fact]
    public void Leaves_Out_What_The_Message_Does_Not_Have()
    {
        var html = BasicEmailMessageRenderer.Instance.Render(new EmailMessageModel { Subject = "S" });
        Assert.DoesNotContain("<a ", html, StringComparison.Ordinal);
        Assert.DoesNotContain("<p>", html, StringComparison.Ordinal);
    }
}
