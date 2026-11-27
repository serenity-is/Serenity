namespace Serenity.Extensions;

/// <summary>
/// Lists and ends the sessions of the users, see <see cref="IAccountSessionStore"/>. Ending sessions tells the account event
/// handlers (<see cref="AccountEventKind.SessionsRevoked"/>), and, but for a new sign-in that replaces the others, revokes
/// the other tokens of the user too (<see cref="IAccountTokenRevoker"/>).
/// </summary>
public interface IAccountSessionManager
{
    /// <summary>Whether the sessions are tracked, i.e. they are enabled and there is a store.</summary>
    bool IsEnabled { get; }

    /// <summary>Lists the active sessions of a user, the newest first.</summary>
    /// <param name="userId">The ID of the user.</param>
    List<AccountSession> ListActive(string userId);

    /// <summary>Ends a session of a user, and tells whether it did.</summary>
    /// <param name="userId">The ID of the user, which the session must be of.</param>
    /// <param name="sessionId">The ID of the session.</param>
    /// <param name="reason">Why.</param>
    bool End(string userId, string sessionId, AccountSessionEndReason reason);

    /// <summary>Ends the sessions of a user, but one, and tells how many it ended.</summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="reason">Why.</param>
    /// <param name="exceptSessionId">The session that goes on, e.g. the current one, if any.</param>
    int EndAll(string userId, AccountSessionEndReason reason, string? exceptSessionId = null);
}
