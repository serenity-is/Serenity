using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Serenity.Extensions;

/// <summary>
/// Ties the sign-in cookies to the sessions of the users (<see cref="IAccountSessionStore"/>): a sign-in starts a session,
/// whose ID the cookie has as a claim; the session is checked at most every <see cref="AccountSessionOptions.ValidationMinutes"/>,
/// and a cookie of a session that ended, or of a user who is no longer active, is signed out; a sign-out ends the session.
/// A cookie of before the sessions gets one when it is checked. <see cref="AccountSessionCookieSetup"/> calls it from the
/// events of the cookie scheme.
/// </summary>
/// <param name="options">The session options.</param>
/// <param name="userRetriever">Gives the users.</param>
/// <param name="sessions">Ends the other sessions of a user, for <see cref="AccountSessionConcurrency.SignOutOthers"/>.</param>
/// <param name="store">The store, without which the sessions are not tracked.</param>
/// <param name="logger">Logs the errors of the store, which don't fail the requests.</param>
/// <param name="timeProvider">Gives the time.</param>
public class AccountSessionCookieValidator(IOptionsMonitor<AccountSessionOptions> options,
    IUserRetrieveService userRetriever,
    IAccountSessionManager sessions,
    IAccountSessionStore? store = null,
    ILogger<AccountSessionCookieValidator>? logger = null,
    TimeProvider? timeProvider = null)
{
    /// <summary>The item of the properties of the cookie with the time its session was last checked.</summary>
    public const string ValidatedItem = ".s-session-validated";

    private DateTime UtcNow => (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;

    private bool IsEnabled => store != null && options.CurrentValue.Enabled;

    /// <summary>
    /// Starts a session for a sign-in, ending the one of the cookie it replaces, if any, and the other sessions of the user
    /// when only one is allowed.
    /// </summary>
    /// <param name="context">The context of the sign-in.</param>
    public virtual void SigningIn(CookieSigningInContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!IsEnabled || context.Principal?.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
            return;

        try
        {
            var now = UtcNow;
            var user = string.IsNullOrEmpty(identity.Name) ? null : userRetriever.ByUsername(identity.Name);

            // a return from an impersonation signs in with the identity of the impersonator, whose session goes on
            var existing = identity.FindFirst(AccountSessionClaimTypes.SessionId)?.Value;
            var resumed = existing != null && user != null && store!.Get(existing) is { EndedDate: null } own &&
                string.Equals(own.UserId, user.Id, StringComparison.Ordinal);

            // the cookie of another sign-in in this browser, e.g. of another user, ends with it, but not the session of an
            // impersonator, who returns to it
            var impersonatorSession = identity.Actor?.FindFirst(AccountSessionClaimTypes.SessionId)?.Value;
            if (context.HttpContext.User.GetSessionId() is string previous && previous != existing && previous != impersonatorSession)
                store!.End(previous, AccountSessionEndReason.SignedOut, now);

            if (user == null)
                return;

            context.Properties.Items[ValidatedItem] = now.Ticks.ToString(CultureInfo.InvariantCulture);
            if (resumed)
                return;

            var session = StartSession(identity, user, context.HttpContext, now);

            // an impersonation doesn't sign the user out elsewhere
            if (identity.Actor == null && options.CurrentValue.Concurrent == AccountSessionConcurrency.SignOutOthers)
                sessions.EndAll(user.Id, AccountSessionEndReason.Replaced, session.SessionId);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "The session of the sign-in of {Username} could not be started.", identity.Name);
        }
    }

    /// <summary>
    /// Checks the session of a cookie, when it was not checked lately, and signs it out when the session ended or its user
    /// is not active, else notes that it is used and renews the cookie with the time it was checked.
    /// </summary>
    /// <param name="context">The context of the check.</param>
    public virtual async Task ValidatePrincipalAsync(CookieValidatePrincipalContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!IsEnabled || context.Principal?.Identity is not ClaimsIdentity { IsAuthenticated: true } identity)
            return;

        var now = UtcNow;
        bool reject;
        try
        {
            var sessionId = identity.FindFirst(AccountSessionClaimTypes.SessionId)?.Value;
            if (string.IsNullOrEmpty(sessionId))
                reject = !AdoptCookie(context, identity, now);
            else if (context.Properties.Items.TryGetValue(ValidatedItem, out var text) &&
                long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var ticks) &&
                now - new DateTime(ticks, DateTimeKind.Utc) < TimeSpan.FromMinutes(Math.Max(0, options.CurrentValue.ValidationMinutes)))
                return;
            else
                reject = !CheckSession(context, identity, sessionId, now);
        }
        catch (Exception ex)
        {
            // the store is not available, e.g. its database is down, which doesn't sign out everyone
            logger?.LogError(ex, "The session of the sign-in of {Username} could not be checked.", identity.Name);
            return;
        }

        if (reject)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(context.Scheme.Name).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Ends the session of a cookie that is signed out, and the one of its impersonator, if any.
    /// </summary>
    /// <param name="context">The context of the sign-out.</param>
    public virtual void SigningOut(CookieSigningOutContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!IsEnabled || context.HttpContext.User.GetSessionId() is not string sessionId)
            return;

        try
        {
            var now = UtcNow;
            store!.End(sessionId, AccountSessionEndReason.SignedOut, now);
            // signing out while impersonating signs the impersonator out too, whose session the cookie has
            if (context.HttpContext.User.GetImpersonator()?.FindFirst(AccountSessionClaimTypes.SessionId)?.Value is string impersonatorSession)
                store.End(impersonatorSession, AccountSessionEndReason.SignedOut, now);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "The session {SessionId} could not be ended.", sessionId);
        }
    }

    /// <summary>
    /// Checks a session, and notes that it is used, unless it ended or its user is not active.
    /// </summary>
    /// <returns>True when the cookie is fine.</returns>
    protected virtual bool CheckSession(CookieValidatePrincipalContext context, ClaimsIdentity identity, string sessionId, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(identity);
        var session = store!.Get(sessionId);
        if (session == null || session.EndedDate != null)
            return false;

        // an impersonation ends with the session of its impersonator, e.g. signed out everywhere
        if (identity.Actor?.FindFirst(AccountSessionClaimTypes.SessionId)?.Value is string impersonatorSession &&
            store.Get(impersonatorSession) is not { EndedDate: null })
        {
            store.End(sessionId, AccountSessionEndReason.SignedOut, now);
            return false;
        }

        // the cookie is of the user of the session, who is still active
        var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = userRetriever.ById(session.UserId);
        if (user == null || user.IsActive != 1 || (userId != null && !string.Equals(userId, session.UserId, StringComparison.Ordinal)))
        {
            store.End(sessionId, AccountSessionEndReason.Deactivated, now);
            return false;
        }

        store.Touch(sessionId, now, context.HttpContext.Connection.RemoteIpAddress?.ToString());
        context.Properties.Items[ValidatedItem] = now.Ticks.ToString(CultureInfo.InvariantCulture);
        context.ShouldRenew = true;
        return true;
    }

    /// <summary>
    /// Starts a session for a cookie of before the sessions, so it is tracked from now on without signing in again.
    /// </summary>
    /// <returns>True when the cookie is fine, false when its user is not there or not active.</returns>
    protected virtual bool AdoptCookie(CookieValidatePrincipalContext context, ClaimsIdentity identity, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(identity);
        var user = string.IsNullOrEmpty(identity.Name) ? null : userRetriever.ByUsername(identity.Name);
        if (user == null || user.IsActive != 1)
            return false;

        StartSession(identity, user, context.HttpContext, now);
        context.Properties.Items[ValidatedItem] = now.Ticks.ToString(CultureInfo.InvariantCulture);
        context.ReplacePrincipal(new ClaimsPrincipal(identity));
        context.ShouldRenew = true;
        return true;
    }

    /// <summary>
    /// Adds a session for a user to the store, and its ID to the claims of the identity.
    /// </summary>
    protected virtual AccountSession StartSession(ClaimsIdentity identity, IUserDefinition user, HttpContext httpContext, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(httpContext);
        foreach (var claim in identity.FindAll(AccountSessionClaimTypes.SessionId).ToArray())
            identity.RemoveClaim(claim);

        var session = new AccountSession
        {
            SessionId = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant(),
            UserId = user.Id,
            Username = user.Username,
            CreatedDate = now,
            LastSeenDate = now,
            Method = identity.AuthenticationType,
            ImpersonatorUserId = identity.Actor?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            IPAddress = httpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = httpContext.Request.Headers.UserAgent.ToString() is { Length: > 0 } agent ? agent : null
        };
        store!.Create(session);
        identity.AddClaim(new Claim(AccountSessionClaimTypes.SessionId, session.SessionId));
        return session;
    }
}
