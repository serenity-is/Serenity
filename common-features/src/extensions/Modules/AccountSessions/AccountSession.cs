namespace Serenity.Extensions;

/// <summary>
/// A session of a user, from a sign-in to its end, which the sign-in cookie refers to by <see cref="SessionId"/>, see
/// <see cref="IAccountSessionStore"/>. The times are in UTC.
/// </summary>
public class AccountSession
{
    /// <summary>The ID of the session, a random text that the cookie has as a claim.</summary>
    public string SessionId { get; set; } = "";

    /// <summary>The ID of the user.</summary>
    public string UserId { get; set; } = "";

    /// <summary>The username of the user when the session began.</summary>
    public string? Username { get; set; }

    /// <summary>When the user signed in.</summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>When the session was last used, at most a validation interval ago while it is used.</summary>
    public DateTime LastSeenDate { get; set; }

    /// <summary>When the session ended, or null while it lasts.</summary>
    public DateTime? EndedDate { get; set; }

    /// <summary>Why the session ended.</summary>
    public AccountSessionEndReason? EndReason { get; set; }

    /// <summary>How the user signed in, the authentication type, e.g. "Password" or "Google".</summary>
    public string? Method { get; set; }

    /// <summary>The address of the client when it was last seen.</summary>
    public string? IPAddress { get; set; }

    /// <summary>The user agent of the client.</summary>
    public string? UserAgent { get; set; }

    /// <summary>The ID of the user who impersonates the user in this session, if any.</summary>
    public string? ImpersonatorUserId { get; set; }
}
