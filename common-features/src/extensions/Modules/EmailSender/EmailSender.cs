using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using System.IO;

namespace Serenity.Extensions;

public class EmailSender(IWebHostEnvironment host, IOptions<SmtpSettings> settings,
    IEmailQueue emailQueue = null) : IEmailSender
{
    private readonly IWebHostEnvironment host = host ?? throw new ArgumentNullException(nameof(host));
    private readonly SmtpSettings settings = (settings ?? throw new ArgumentNullException(nameof(settings))).Value;

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