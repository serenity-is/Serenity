using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Serenity.Extensions;

/// <summary>
/// The default <see cref="IAccountEventPublisher"/>, which fills the user of an event from its ID, who did it from the
/// current user, and the time, IP address and user agent of the request, and tells the handlers one after another, in the
/// request, so they can use its services, e.g. to render an email. An exception of a handler is logged.
/// </summary>
/// <param name="handlers">The handlers of the events.</param>
/// <param name="userRetriever">Gives the users from their IDs.</param>
/// <param name="userAccessor">Gives the current user, who did it.</param>
/// <param name="httpContextAccessor">Gives the request, for its IP address and user agent.</param>
/// <param name="logger">Logs the exceptions of the handlers.</param>
/// <param name="timeProvider">Gives the time.</param>
public class AccountEventPublisher(IEnumerable<IAccountEventHandler> handlers,
    IUserRetrieveService userRetriever,
    IUserAccessor userAccessor,
    IHttpContextAccessor? httpContextAccessor = null,
    ILogger<AccountEventPublisher>? logger = null,
    TimeProvider? timeProvider = null) : IAccountEventPublisher
{
    private readonly IAccountEventHandler[] handlers = handlers?.ToArray() ?? [];

    /// <inheritdoc/>
    public void Publish(AccountEvent accountEvent, IUnitOfWork? unitOfWork = null)
    {
        ArgumentNullException.ThrowIfNull(accountEvent);
        if (handlers.Length == 0)
            return;

        // what the request tells is taken now, and the users are read after the commit, as the transaction of the change
        // may lock the row of the user, which the user retrieve service reads with a connection of its own, and would wait
        // for until the command times out, blocking the others that read the users meanwhile
        PrepareRequest(accountEvent);
        if (unitOfWork != null)
            unitOfWork.OnCommit += () =>
            {
                PrepareUsers(accountEvent);
                Dispatch(accountEvent);
            };
        else
        {
            PrepareUsers(accountEvent);
            Dispatch(accountEvent);
        }
    }

    /// <summary>
    /// Fills what the request tells: who did it, from the claims of the current user, when, and from where. It doesn't read
    /// the database, as it is called in the transaction of the change.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual void PrepareRequest(AccountEvent accountEvent)
    {
        if (accountEvent.Time == default)
            accountEvent.Time = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;

        // who is responsible, the impersonator of an impersonated user
        var principal = userAccessor.User;
        if (accountEvent.ActorUserId == null && principal?.Identity?.IsAuthenticated == true)
        {
            accountEvent.ActorUserId = principal.GetAuditIdentifier();
            accountEvent.ActorUsername ??= principal.GetImpersonator()?.Name ?? principal.Identity.Name;
        }

        var context = httpContextAccessor?.HttpContext;
        if (context != null)
        {
            accountEvent.IPAddress ??= context.Connection.RemoteIpAddress?.ToString();
            accountEvent.UserAgent ??= context.Request.Headers.UserAgent.ToString() is { Length: > 0 } agent ? agent : null;
        }
    }

    /// <summary>
    /// Fills the user of the event from its ID, and the display name of who did it, which are read from the user retrieve
    /// service, after the transaction of the change is committed.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual void PrepareUsers(AccountEvent accountEvent)
    {
        if (!string.IsNullOrEmpty(accountEvent.UserId) &&
            (accountEvent.Username == null || accountEvent.DisplayName == null || accountEvent.Email == null))
        {
            var user = userRetriever.ById(accountEvent.UserId);
            if (user != null)
            {
                accountEvent.Username ??= user.Username;
                accountEvent.DisplayName ??= user.DisplayName;
                accountEvent.Email ??= user.Email;
            }
        }

        if (accountEvent.ActorDisplayName == null && !string.IsNullOrEmpty(accountEvent.ActorUserId))
            accountEvent.ActorDisplayName = userRetriever.ById(accountEvent.ActorUserId)?.DisplayName ?? accountEvent.ActorUsername;
    }

    /// <summary>
    /// Tells the handlers of the event, logging the exceptions they throw.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual void Dispatch(AccountEvent accountEvent)
    {
        foreach (var handler in handlers)
        {
            try
            {
                handler.HandleAsync(accountEvent).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "The account event handler {Handler} failed for {Kind} of the user {UserId}.",
                    handler.GetType().Name, accountEvent.Kind, accountEvent.UserId);
            }
        }
    }
}
