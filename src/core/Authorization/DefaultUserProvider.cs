namespace Serenity.Services;

/// <summary>
/// Default implementation of <see cref="IUserProvider"/> that delegates to an <see cref="IUserAccessor"/>,
/// <see cref="IUserClaimCreator"/>, and <see cref="IUserRetrieveService"/>.
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="DefaultUserProvider"/> class.
/// </remarks>
/// <param name="userAccessor">The accessor that provides the current user principal.</param>
/// <param name="userClaimCreator">The service used to create principals for impersonation.</param>
/// <param name="userRetriever">The service used to retrieve user definitions.</param>
/// <param name="cache">The two-level cache used for fallback invalidation when the retrieve service does not implement <see cref="IRemoveCachedUser"/>. Optional.</param>
/// <exception cref="ArgumentNullException"><paramref name="userAccessor"/>, <paramref name="userClaimCreator"/> or <paramref name="userRetriever"/> is <c>null</c>.</exception>
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