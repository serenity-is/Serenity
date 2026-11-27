namespace Serenity.Extensions;

/// <summary>
/// Options of the sessions of the users, see <c>AddAccountSessions</c>.
/// </summary>
[DefaultSectionKey(SectionKey)]
public class AccountSessionOptions
{
    /// <summary>
    /// The configuration section key of the session options.
    /// </summary>
    public const string SectionKey = "AccountSessions";

    /// <summary>
    /// Whether the sessions are tracked and checked. True by default.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// How often, in minutes, the session of a sign-in cookie is checked, so a session that is signed out elsewhere, or a
    /// user who is deactivated, is signed out within this time. 1 by default.
    /// </summary>
    public double ValidationMinutes { get; set; } = 1;

    /// <summary>
    /// The minutes after which a session that was not used is no longer listed as active. The expiration of the sign-in
    /// cookie when not set, as its cookie is expired then.
    /// </summary>
    public double? IdleMinutes { get; set; }

    /// <summary>
    /// Whether a user can have more than one session at a time. <see cref="AccountSessionConcurrency.Allow"/> by default.
    /// </summary>
    public AccountSessionConcurrency Concurrent { get; set; }

    /// <summary>
    /// The number of days the sessions that ended are kept. Zero keeps them forever. 90 by default.
    /// </summary>
    public int RetentionDays { get; set; } = 90;
}
