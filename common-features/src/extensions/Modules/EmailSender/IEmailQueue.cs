using MimeKit;

namespace Serenity.Extensions;

/// <summary>
/// Abstraction for an email queue that stores messages to be sent later.
/// </summary>
public interface IEmailQueue
{
    /// <summary>
    /// Enqueues the specified email message.
    /// </summary>
    /// <param name="message">The email message to enqueue.</param>
    void Enqueue(MimeMessage message);
}