using MailKit.Security;

namespace Serenity.Extensions;

/// <summary>
/// Settings for SMTP email sending.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class SmtpSettings
{
    /// <summary>
    /// Default section key for SMTP settings.
    /// </summary>
    public const string SectionKey = "SmtpSettings";

    /// <summary>
    /// The SMTP server host.
    /// </summary>
    public string Host { get; set; }
    /// <summary>
    /// The SMTP server port.
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// The secure socket options used when connecting to the SMTP server.
    /// </summary>
    public SecureSocketOptions SecureSocket { get; set; }
    /// <summary>
    /// The username used to authenticate with the SMTP server.
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// The password used to authenticate with the SMTP server.
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// The default sender address used when a message has no From address.
    /// </summary>
    public string From { get; set; }
    /// <summary>
    /// The folder where outgoing emails are written when no SMTP host is configured.
    /// </summary>
    public string PickupPath { get; set; }
    /// <summary>
    /// Whether to automatically enqueue emails instead of sending them directly.
    /// </summary>
    public bool AutoUseQueue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SmtpSettings"/> class.
    /// </summary>
    public SmtpSettings()
    {
        SecureSocket = SecureSocketOptions.Auto;
    }
}