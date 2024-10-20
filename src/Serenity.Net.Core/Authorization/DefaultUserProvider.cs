namespace Serenity.Services;

/// <summary>
/// Default implementation for <see cref="IUserProvider"/> which is a combination of IUserAccessor and IUserRetrieveService.
/// </summary>
/// <param name="userAccessor">User accessor</param>
/// <param name="userClaimCreator">User claim creator</param>
/// <param name="userRetriever">User retrieve service</param>
/// <param name="cache">Optional cache for graceful invalidation if the IUserRetrieveService does not implement IUser</param>
/// <exception cref="ArgumentNullException"></exception>
public class DefaultUserProvider(IUserAccessor userAccessor, IUserClaimCreator userClaimCreator, IUserRetrieveService userRetriever,
    ITwoLevelCache? cache = null) : IUserProvider
{
    private readonly IUserAccessor userAccessor = userAccessor ?? throw new ArgumentNullException(nameof(userAccessor));
    private readonly IUserClaimCreator userClaimCreator = userClaimCreator ?? throw new ArgumentNullException(nameof(userClaimCreator));
    private readonly IUserRetrieveService userRetriever = userRetriever ?? throw new ArgumentNullException(nameof(userRetriever));

    /// <inheritdoc/>
    public IUserDefinition? ById(string id)
    {
        return userRetriever.ById(id);
    }

    /// <inheritdoc/>
    public IUserDefinition? ByUsername(string username)
    {
        return userRetriever.ByUsername(username);
    }

    /// <inheritdoc/>
    public ClaimsPrincipal CreatePrincipal(string username, string authType)
    {
        return userClaimCreator.CreatePrincipal(username, authType);
    }

    private IImpersonator Impersonator => userAccessor as IImpersonator 
        ?? throw new InvalidOperationException("UserAccessor is not an impersonator, e.g. it does not implement IImpersonator!");

    /// <inheritdoc/>
    public void Impersonate(ClaimsPrincipal user)
    {
        Impersonator.Impersonate(user);
    }

    /// <inheritdoc/>
    public void UndoImpersonate()
    {
        Impersonator.UndoImpersonate();
    }

    /// <inheritdoc/>
    public void RemoveAll()
    {
        if (userRetriever is IRemoveAll removeAll)
            removeAll.RemoveAll();
        else
            cache?.ExpireGroupItems("Default.Users");
    }

    /// <inheritdoc/>
    public void RemoveCachedUser(string? userId, string? username)
    {
        userRetriever.RemoveCachedUser(userId, username, cache);
    }

    /// <inheritdoc/>
    public ClaimsPrincipal? User => userAccessor.User;
}