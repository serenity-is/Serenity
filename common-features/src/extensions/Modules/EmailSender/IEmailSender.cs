using MimeKit;

namespace Serenity.Extensions;

/// <summary>
/// Abstraction for sending email messages.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends the specified email message.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <param name="skipQueue">Whether to bypass the email queue and send directly.</param>
    void Send(MimeMessage message, bool skipQueue = false);
}