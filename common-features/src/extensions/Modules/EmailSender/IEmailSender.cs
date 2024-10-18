using MimeKit;

namespace Serenity.Extensions;

public interface IEmailSender
{
    void Send(MimeMessage message, bool skipQueue = false);
}