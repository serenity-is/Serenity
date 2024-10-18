using MimeKit;

namespace Serenity.Extensions;

public static class EmailSenderExtensions
{
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