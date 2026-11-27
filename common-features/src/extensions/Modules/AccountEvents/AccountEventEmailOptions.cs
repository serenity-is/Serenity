namespace Serenity.Extensions;

/// <summary>
/// Options of <see cref="AccountEventEmailHandler"/>, which emails the users when their account changes, read from the
/// "AccountEventEmails" section of the settings.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class AccountEventEmailOptions
{
    /// <summary>
    /// The default section key in appsettings.json
    /// </summary>
    public const string SectionKey = "AccountEventEmails";

    /// <summary>
    /// Whether the users are emailed. Default is true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The kinds of the events that are not emailed, e.g. "UsernameChanged, TwoFactorEnabled", separated by commas,
    /// semicolons or white space (see <see cref="SettingsList"/>), and compared ignoring case.
    /// </summary>
    public string? Skip { get; set; }

    /// <summary>
    /// Checks whether the events of a kind are emailed.
    /// </summary>
    /// <param name="kind">The kind.</param>
    public bool IsEnabledFor(AccountEventKind kind)
    {
        if (!Enabled)
            return false;

        foreach (var skip in SettingsList.Split(Skip))
            if (string.Equals(skip, kind.ToString(), StringComparison.OrdinalIgnoreCase))
                return false;

        return true;
    }
}
