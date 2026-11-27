namespace Serenity.Extensions;

/// <summary>
/// Something that happened to an account, e.g. its password was changed, which <see cref="IAccountEventPublisher"/> tells
/// the <see cref="IAccountEventHandler"/>s of, e.g. one that emails the user. The publisher fills what the code that
/// raises it leaves out: the user from <see cref="UserId"/>, who did it, when, and from where.
/// </summary>
public class AccountEvent
{
    /// <summary>What happened.</summary>
    public AccountEventKind Kind { get; set; }

    /// <summary>The ID of the user whose account it is.</summary>
    public string? UserId { get; set; }

    /// <summary>The username of the user.</summary>
    public string? Username { get; set; }

    /// <summary>The display name of the user.</summary>
    public string? DisplayName { get; set; }

    /// <summary>The email address of the user.</summary>
    public string? Email { get; set; }

    /// <summary>The value before, e.g. the old email address of <see cref="AccountEventKind.EmailChanged"/>.</summary>
    public string? OldValue { get; set; }

    /// <summary>The value after, e.g. the new email address of <see cref="AccountEventKind.EmailChanged"/>.</summary>
    public string? NewValue { get; set; }

    /// <summary>The key of the two-factor authentication method, e.g. "Email", for the two-factor events.</summary>
    public string? Method { get; set; }

    /// <summary>The ID of the user who did it, which is not set when no one was signed in, e.g. for a reset link.</summary>
    public string? ActorUserId { get; set; }

    /// <summary>The username of the user who did it.</summary>
    public string? ActorUsername { get; set; }

    /// <summary>The display name of the user who did it.</summary>
    public string? ActorDisplayName { get; set; }

    /// <summary>Whether the user did it, or no one who was signed in, e.g. with a reset link that was emailed to the user.</summary>
    public bool IsSelf => string.IsNullOrEmpty(ActorUserId) || string.Equals(ActorUserId, UserId, StringComparison.Ordinal);

    /// <summary>When it happened, in UTC.</summary>
    public DateTime Time { get; set; }

    /// <summary>The IP address of the request.</summary>
    public string? IPAddress { get; set; }

    /// <summary>The user agent of the request, e.g. the browser.</summary>
    public string? UserAgent { get; set; }
}
