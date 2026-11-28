namespace Serenity.Extensions;

/// <summary>
/// Renders the HTML of an email that code builds, e.g. an invitation or a two-factor code, in the layout of the emails.
/// <c>AddEmailSender</c> registers <see cref="RazorEmailMessageRenderer"/>.
/// </summary>
public interface IEmailMessageRenderer
{
    /// <summary>
    /// Renders the HTML of an email.
    /// </summary>
    /// <param name="message">The email.</param>
    string Render(EmailMessageModel message);
}
