namespace Serenity.Extensions;

/// <summary>
/// The model of the email that tells a user that the account changed, see <see cref="AccountEventEmailHandler"/>.
/// </summary>
public class AccountEventEmailModel
{
    /// <summary>The event.</summary>
    public AccountEvent Event { get; set; } = new();

    /// <summary>The subject, which is also the title of the email.</summary>
    public string Subject { get; set; } = "";

    /// <summary>What changed, as a sentence.</summary>
    public string Message { get; set; } = "";

    /// <summary>The name of the site, e.g. for the signature.</summary>
    public string SiteTitle { get; set; } = "";
}
