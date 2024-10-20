namespace Serenity.Extensions;

/// <summary>
/// Base user retrieve service that provides common functionality for user retrieve services.
/// </summary>
/// <param name="cache">Cache</param>
public abstract class BaseUserRetrieveService(ITwoLevelCache cache) : IUserRetrieveService, IRemoveCachedUser, IRemoveAll
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));

    /// <inheritdoc/>
    public virtual IUserDefinition ById(string id)
    {
        if (!IsValidUserId(id))
            return null;

        return GetCachedById(id);
    }

    /// <inheritdoc/>
    public virtual IUserDefinition ByUsername(string username)
    {
        if (!IsValidUsername(username))
            return null;

        return GetCachedByUsername(username);
    }

    /// <summary>
    /// Gets the cache duration for user retrieval. Default is zero, meaning it will be 
    /// cached indefinitely unless expired by using the cache group key.
    /// </summary>
    protected virtual TimeSpan GetCacheDuration() => TimeSpan.Zero;

    /// <summary>
    /// Gets the cache group key for user retrieval.
    /// </summary>
    protected abstract string GetCacheGroupKey();

    /// <summary>
    /// Gets the cache key for the specified user ID.
    /// </summary>
    /// <param name="id">User ID</param>
    protected virtual string GetIdCacheKey(string id) => "UserByID_" + id;

    /// <summary>
    /// Gets the cache key for the specified username.
    /// </summary>
    /// <param name="username">Username</param>
    protected virtual string GetUsernameCacheKey(string username) => "UserByName_" + username.ToLowerInvariant();

    /// <summary>
    /// Gets the cached user by the specified ID.
    /// </summary>
    /// <param name="id">User ID</param>
    protected virtual IUserDefinition GetCachedById(string id)
    {
        return cache.GetLocalStoreOnly(GetIdCacheKey(id), GetCacheDuration(), GetCacheGroupKey(),
            () => LoadById(id));
    }

    /// <summary>
    /// Gets the cached user by the specified username.
    /// </summary>
    /// <param name="username">Username</param>
    protected virtual IUserDefinition GetCachedByUsername(string username)
    {
        return cache.GetLocalStoreOnly(GetUsernameCacheKey(username), GetCacheDuration(), GetCacheGroupKey(),
            () => LoadByUsername(username));
    }

    /// <summary>
    /// Checks if the specified user ID is valid. By default, it checks if it is not null or empty.
    /// </summary>
    /// <param name="userId">User ID</param>
    protected virtual bool IsValidUserId(string userId) => !string.IsNullOrEmpty(userId);

    /// <summary>
    /// Checks if the specified username is valid. By default, it checks if it is not null or empty.
    /// </summary>
    /// <param name="username">Username</param>
    protected virtual bool IsValidUsername(string username) => !string.IsNullOrEmpty(username);

    /// <summary>
    /// Loads the user by the specified ID from database.
    /// </summary>
    /// <param name="id">User ID</param>
    protected abstract IUserDefinition LoadById(string id);

    /// <summary>
    /// Loads the user by the specified username from database
    /// </summary>
    /// <param name="username">Username</param>
    protected abstract IUserDefinition LoadByUsername(string username);

    /// <inheritdoc/>
    public virtual void RemoveAll()
    {
        cache.ExpireGroupItems(GetCacheGroupKey());
    }

    /// <inheritdoc/>
    public virtual void RemoveCachedUser(string userId, string username)
    {
        if (IsValidUserId(userId))
            cache.Remove(GetIdCacheKey(userId));

        if (IsValidUsername(username))
            cache.Remove(GetUsernameCacheKey(username));
    }
}