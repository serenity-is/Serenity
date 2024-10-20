namespace Serenity.Extensions;

public abstract class BaseUserRetrieveService(ITwoLevelCache cache) : IUserRetrieveService, IRemoveCachedUser, IRemoveAll
{
    private readonly ITwoLevelCache cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public virtual IUserDefinition ById(string id)
    {
        if (!IsValidUserId(id))
            return null;

        return GetCachedById(id);
    }

    public virtual IUserDefinition ByUsername(string username)
    {
        if (!IsValidUsername(username))
            return null;

        return GetCachedByUsername(username);
    }

    protected virtual TimeSpan GetCacheDuration() => TimeSpan.Zero;
    protected abstract string GetCacheGroupKey();

    protected virtual string GetIdCacheKey(string id) => "UserByID_" + id;
    protected virtual string GetUsernameCacheKey(string username) => "UserByName_" + username.ToLowerInvariant();

    protected virtual IUserDefinition GetCachedById(string id)
    {
        return cache.GetLocalStoreOnly(GetIdCacheKey(id), GetCacheDuration(), GetCacheGroupKey(),
            () => LoadById(id));
    }

    protected virtual IUserDefinition GetCachedByUsername(string username)
    {
        return cache.GetLocalStoreOnly(GetUsernameCacheKey(username), GetCacheDuration(), GetCacheGroupKey(),
            () => LoadByUsername(username));
    }

    protected virtual bool IsValidUserId(string userId) => !string.IsNullOrEmpty(userId);

    protected virtual bool IsValidUsername(string username) => !string.IsNullOrEmpty(username);

    protected abstract IUserDefinition LoadById(string id);
    protected abstract IUserDefinition LoadByUsername(string username);

    public virtual void RemoveAll()
    {
        cache.ExpireGroupItems(GetCacheGroupKey());
    }

    public virtual void RemoveCachedUser(string userId, string username)
    {
        if (IsValidUserId(userId))
            cache.Remove(GetIdCacheKey(userId));

        if (IsValidUsername(username))
            cache.Remove(GetUsernameCacheKey(username));
    }
}