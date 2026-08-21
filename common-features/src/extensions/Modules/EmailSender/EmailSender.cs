using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System.IO;

namespace Serenity.Extensions;

/// <summary>
/// Default implementation of <see cref="IEmailSender"/> that sends emails
/// via SMTP, a pickup folder, or an email queue.
/// </summary>
public class EmailSender(IWebHostEnvironment host, IOptions<SmtpSettings> settings,
    IEmailQueue emailQueue = null) : IEmailSender
{
    private readonly IWebHostEnvironment host = host ?? throw new ArgumentNullException(nameof(host));
    private readonly SmtpSettings settings = (settings ?? throw new ArgumentNullException(nameof(settings))).Value;

    /// <summary>
    /// Sends the specified email message, either directly, via the configured
    /// pickup folder, or by enqueuing it when queueing is enabled.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <param name="skipQueue">Whether to bypass the email queue and send directly.</param>
    public void Send(MimeMessage message, bool skipQueue)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.From.Count == 0 && !string.IsNullOrEmpty(settings.From))
            message.From.Add(MailboxAddress.Parse(settings.From));

        if (!skipQueue && settings.AutoUseQueue && emailQueue != null)
        {
            emailQueue.Enqueue(message);
        }
        else if (!string.IsNullOrEmpty(settings.Host))
        {
            using var client = new SmtpClient();
            client.Connect(settings.Host, settings.Port, settings.SecureSocket);
            if (!string.IsNullOrEmpty(settings.Username))
                client.Authenticate(settings.Username, settings.Password);

            client.Send(message);
            client.Disconnect(true);
        }
        else
        {
            var pickupPath = string.IsNullOrEmpty(settings.PickupPath) ?
                Path.Combine(host.ContentRootPath, "App_Data", "Mail") : 
                Path.Combine(host.ContentRootPath, settings.PickupPath);
            if (!Directory.Exists(pickupPath))
                Directory.CreateDirectory(pickupPath);
            message.WriteTo(Path.Combine(pickupPath, DateTime.Now.ToString("yyyyMMdd_HHmmss_fff", 
                CultureInfo.InvariantCulture) + ".eml"));
        }
    }
}