using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Serenity.Extensions;

/// <summary>
/// The default <see cref="IAccountSessionManager"/>.
/// </summary>
/// <param name="options">The session options.</param>
/// <param name="cookieOptions">The options of the sign-in cookie, for its expiration.</param>
/// <param name="revokers">Revoke the other tokens of a user.</param>
/// <param name="store">The store, without which the sessions are not tracked.</param>
/// <param name="accountEvents">Tells the account event handlers.</param>
/// <param name="timeProvider">Gives the time.</param>
public class AccountSessionManager(IOptionsMonitor<AccountSessionOptions> options,
    IOptionsMonitor<CookieAuthenticationOptions> cookieOptions,
    IEnumerable<IAccountTokenRevoker> revokers,
    IAccountSessionStore? store = null,
    IAccountEventPublisher? accountEvents = null,
    TimeProvider? timeProvider = null) : IAccountSessionManager
{
    private DateTime UtcNow => (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;

    /// <inheritdoc/>
    public virtual bool IsEnabled => store != null && options.CurrentValue.Enabled;

    /// <summary>
    /// The time after which a session that was not used is not active, from the options or the sign-in cookie.
    /// </summary>
    protected virtual TimeSpan IdleTimeout => options.CurrentValue.IdleMinutes is double minutes ? TimeSpan.FromMinutes(minutes) :
        cookieOptions.Get(CookieAuthenticationDefaults.AuthenticationScheme).ExpireTimeSpan;

    /// <inheritdoc/>
    public virtual List<AccountSession> ListActive(string userId)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        return IsEnabled ? store!.ListActive(userId, UtcNow - IdleTimeout) : [];
    }

    /// <inheritdoc/>
    public virtual bool End(string userId, string sessionId, AccountSessionEndReason reason)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        if (!IsEnabled)
            return false;

        var session = store!.Get(sessionId);
        if (session == null || session.EndedDate != null || !string.Equals(session.UserId, userId, StringComparison.Ordinal) ||
            !store.End(sessionId, reason, UtcNow))
            return false;

        Publish(userId, reason, 1);
        return true;
    }

    /// <inheritdoc/>
    public virtual int EndAll(string userId, AccountSessionEndReason reason, string? exceptSessionId = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(userId);
        var count = IsEnabled ? store!.EndAll(userId, reason, UtcNow, exceptSessionId) : 0;

        // a new sign-in that replaces the others is not about the security of the account
        if (reason != AccountSessionEndReason.Replaced)
            foreach (var revoker in revokers)
                revoker.RevokeAll(userId);

        if (count > 0)
            Publish(userId, reason, count);
        return count;
    }

    /// <summary>
    /// Tells the account event handlers that sessions of a user ended, e.g. for the activity of the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="reason">Why.</param>
    /// <param name="count">How many.</param>
    protected virtual void Publish(string userId, AccountSessionEndReason reason, int count)
    {
        accountEvents?.Publish(new AccountEvent
        {
            Kind = AccountEventKind.SessionsRevoked,
            UserId = userId,
            Reason = reason.ToString(),
            NewValue = count.ToString(CultureInfo.InvariantCulture)
        });
    }
}
