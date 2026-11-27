namespace Serenity.Extensions;

/// <summary>
/// Tells the <see cref="IAccountEventHandler"/>s of something that happened to an account, e.g. the password was changed.
/// </summary>
public interface IAccountEventPublisher
{
    /// <summary>
    /// Fills what the event leaves out, i.e. the user from its ID, who did it, when, and from where, and tells the handlers,
    /// when the unit of work is committed if one is given, so a change that is rolled back is not told.
    /// </summary>
    /// <param name="accountEvent">The event, with at least its kind and user ID.</param>
    /// <param name="unitOfWork">The unit of work of the change, if any.</param>
    void Publish(AccountEvent accountEvent, IUnitOfWork? unitOfWork = null);
}
