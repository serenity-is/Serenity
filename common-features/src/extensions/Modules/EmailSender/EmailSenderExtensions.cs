using MimeKit;

namespace Serenity.Extensions;

/// <summary>
/// Extension methods for <see cref="IEmailSender"/>.
/// </summary>
public static class EmailSenderExtensions
{
    /// <summary>
    /// Sends a simple HTML email with the specified subject, body, and recipient.
    /// </summary>
    /// <param name="emailSender">The email sender.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The HTML body of the email.</param>
    /// <param name="mailTo">The recipient email address.</param>
    public static void Send(this IEmailSender emailSender, string subject, string body, string mailTo)
    {
        var message = new MimeMessage();
        ArgumentNullException.ThrowIfNull(mailTo);
        message.To.Add(MailboxAddress.Parse(mailTo));
        message.Subject = subject;
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = body
        };
        message.Body = bodyBuilder.ToMessageBody();
        emailSender.Send(message);
    }
}