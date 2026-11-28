namespace Serenity.Extensions;

/// <summary>
/// An email that code builds, e.g. an invitation or a two-factor code, which the <c>EmailMessage</c> view shows in the layout
/// of the emails, see <see cref="IEmailMessageRenderer"/>. The texts are plain, and encoded when they are shown.
/// </summary>
public class EmailMessageModel
{
    /// <summary>The subject, which is also the title of the email.</summary>
    public string? Subject { get; set; }

    /// <summary>The greeting, e.g. "Hello Nancy,".</summary>
    public string? Greeting { get; set; }

    /// <summary>The paragraphs of the message.</summary>
    public List<string> Paragraphs { get; set; } = [];

    /// <summary>A code to show large, e.g. the one of a two-factor authentication.</summary>
    public string? Code { get; set; }

    /// <summary>The button, e.g. the link that accepts an invitation.</summary>
    public EmailButtonModel? Button { get; set; }

    /// <summary>The notes after the message, shown smaller, e.g. until when a link works.</summary>
    public List<string> Notes { get; set; } = [];
}
