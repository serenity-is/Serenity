using MailKit.Security;

namespace Serenity.Extensions;

[DefaultSectionKey(SectionKey)]
public class SmtpSettings
{
    public const string SectionKey = "SmtpSettings";

    public string Host { get; set; }
    public int Port { get; set; }
    public SecureSocketOptions SecureSocket { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string PickupPath { get; set; }
    public bool AutoUseQueue { get; set; }

    public SmtpSettings()
    {
        SecureSocket = SecureSocketOptions.Auto;
    }
}