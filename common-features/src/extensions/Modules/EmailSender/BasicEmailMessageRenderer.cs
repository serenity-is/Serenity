using System.Net;

namespace Serenity.Extensions;

/// <summary>
/// Renders an email that code builds as simple HTML, without the layout of the emails, for when there is no
/// <see cref="IEmailMessageRenderer"/>, e.g. outside a web application, or in a test.
/// </summary>
public class BasicEmailMessageRenderer : IEmailMessageRenderer
{
    /// <summary>The shared instance.</summary>
    public static readonly BasicEmailMessageRenderer Instance = new();

    /// <inheritdoc/>
    public virtual string Render(EmailMessageModel message)
    {
        ArgumentNullException.ThrowIfNull(message);
        static string E(string? value) => WebUtility.HtmlEncode(value ?? "");
        var sb = new StringBuilder();
        sb.Append("<html><head><title>").Append(E(message.Subject)).Append("</title></head><body>");
        if (!string.IsNullOrEmpty(message.Greeting))
            sb.Append("<p>").Append(E(message.Greeting)).Append("</p>");
        foreach (var paragraph in message.Paragraphs)
            sb.Append("<p>").Append(E(paragraph)).Append("</p>");
        if (!string.IsNullOrEmpty(message.Code))
            sb.Append("<p><b>").Append(E(message.Code)).Append("</b></p>");
        if (message.Button != null)
            sb.Append("<p><a href=\"").Append(E(message.Button.Url)).Append("\">").Append(E(message.Button.Text)).Append("</a></p>");
        foreach (var note in message.Notes)
            sb.Append("<p>").Append(E(note)).Append("</p>");
        sb.Append("</body></html>");
        return sb.ToString();
    }
}
