using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Serenity.Extensions;

/// <summary>
/// Ends the sessions of a user when the account changes for security: the other sessions when the user changes the
/// password, and all of them when an administrator sets it or a reset link does, or an administrator resets the two-factor
/// authentication. A deactivated user is signed out by <see cref="AccountSessionCookieValidator"/>.
/// </summary>
/// <param name="services">Gives the <see cref="IAccountSessionManager"/> that ends the sessions, when an event is handled,
/// as the manager tells the account event publisher in turn, which needs this handler.</param>
/// <param name="httpContextAccessor">Gives the current session.</param>
public class AccountSessionEventHandler(IServiceProvider services, IHttpContextAccessor? httpContextAccessor = null)
    : IAccountEventHandler
{
    /// <inheritdoc/>
    public virtual Task HandleAsync(AccountEvent accountEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(accountEvent);
        if (string.IsNullOrEmpty(accountEvent.UserId))
            return Task.CompletedTask;

        var reason = accountEvent.Kind switch
        {
            AccountEventKind.PasswordChanged => AccountSessionEndReason.PasswordChanged,
            AccountEventKind.TwoFactorReset => AccountSessionEndReason.TwoFactorReset,
            _ => (AccountSessionEndReason?)null
        };
        if (reason == null)
            return Task.CompletedTask;

        // the session of the user who changed the own password goes on
        var user = httpContextAccessor?.HttpContext?.User;
        var current = reason == AccountSessionEndReason.PasswordChanged &&
            string.Equals(user?.GetIdentifier(), accountEvent.UserId, StringComparison.Ordinal) ? user.GetSessionId() : null;
        services.GetService<IAccountSessionManager>()?.EndAll(accountEvent.UserId, reason.Value, current);
        return Task.CompletedTask;
    }
}
