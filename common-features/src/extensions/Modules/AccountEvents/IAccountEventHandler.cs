namespace Serenity.Extensions;

/// <summary>
/// Handles the events of the accounts, e.g. emails the user that the password was changed, see
/// <see cref="IAccountEventPublisher"/>. Register one with <c>services.AddSingleton&lt;IAccountEventHandler, MyHandler&gt;()</c>,
/// as there can be many. An exception that it throws is logged, and does not undo the change.
/// </summary>
public interface IAccountEventHandler
{
    /// <summary>
    /// Handles an event.
    /// </summary>
    /// <param name="accountEvent">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task HandleAsync(AccountEvent accountEvent, CancellationToken cancellationToken = default);
}
