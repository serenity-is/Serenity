using MimeKit;

namespace Serenity.Extensions;

public interface IEmailQueue
{
    void Enqueue(MimeMessage message);
}