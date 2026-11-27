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

        Prepare(accountEvent);
        if (unitOfWork != null)
            unitOfWork.OnCommit += () => Dispatch(accountEvent);
        else
            Dispatch(accountEvent);
    }

    /// <summary>
    /// Fills what the event leaves out: the user, who did it, when, and from where.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    protected virtual void Prepare(AccountEvent accountEvent)
    {
        if (accountEvent.Time == default)
            accountEvent.Time = (timeProvider ?? TimeProvider.System).GetUtcNow().UtcDateTime;

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

        var principal = userAccessor.User;
        if (accountEvent.ActorUserId == null && principal?.Identity?.IsAuthenticated == true)
        {
            accountEvent.ActorUserId = principal.GetIdentifier();
            accountEvent.ActorUsername ??= principal.Identity.Name;
            accountEvent.ActorDisplayName ??= userRetriever.GetUserDefinition(principal)?.DisplayName ?? principal.Identity.Name;
        }

        var context = httpContextAccessor?.HttpContext;
        if (context != null)
        {
            accountEvent.IPAddress ??= context.Connection.RemoteIpAddress?.ToString();
            accountEvent.UserAgent ??= context.Request.Headers.UserAgent.ToString() is { Length: > 0 } agent ? agent : null;
        }
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
