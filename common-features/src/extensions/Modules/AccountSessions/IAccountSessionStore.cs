namespace Serenity.Extensions;

/// <summary>
/// Stores the sessions of the users, see <see cref="AccountSession"/> and <c>AddAccountSessions</c>. The times are in UTC.
/// </summary>
public interface IAccountSessionStore
{
    /// <summary>Adds a session, when a user signs in.</summary>
    /// <param name="session">The session.</param>
    void Create(AccountSession session);

    /// <summary>Gets a session, ended or not, or null when there is no such session.</summary>
    /// <param name="sessionId">The ID of the session.</param>
    AccountSession? Get(string sessionId);

    /// <summary>Notes that a session is used, with the address of the client.</summary>
    /// <param name="sessionId">The ID of the session.</param>
    /// <param name="lastSeen">The time.</param>
    /// <param name="ipAddress">The address of the client, if known.</param>
    void Touch(string sessionId, DateTime lastSeen, string? ipAddress);

    /// <summary>Ends a session that lasts, and tells whether it did.</summary>
    /// <param name="sessionId">The ID of the session.</param>
    /// <param name="reason">Why.</param>
    /// <param name="endedDate">The time.</param>
    bool End(string sessionId, AccountSessionEndReason reason, DateTime endedDate);

    /// <summary>Ends the sessions of a user that last, but one, and tells how many it ended.</summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="reason">Why.</param>
    /// <param name="endedDate">The time.</param>
    /// <param name="exceptSessionId">The session that goes on, e.g. the current one, if any.</param>
    int EndAll(string userId, AccountSessionEndReason reason, DateTime endedDate, string? exceptSessionId = null);

    /// <summary>Lists the sessions of a user that last and were seen lately, the newest first.</summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="seenSince">The time a session must have been seen since, as the cookie of one that wasn't is expired.</param>
    List<AccountSession> ListActive(string userId, DateTime seenSince);
}
